using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using PatchesApi.V1.Domain;
using PatchesApi.V1.Gateways;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using PatchesApi.V1.Boundary.Request;
using Xunit;
using System.Collections.Generic;
using PatchesApi.V1.Infrastructure;
using PatchesApi.V1.Factories;
using System.Threading;

namespace PatchesApi.Tests.V1.Gateways
{
    [Collection("DynamoDb collection")]
    public class DynamoDbGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly IDynamoDBContext _dynamoDb;
        private PatchesGateway _classUnderTest;
        private readonly List<Action> _cleanup = new List<Action>();


        private Mock<ILogger<PatchesGateway>> _logger;

        public DynamoDbGatewayTests(DynamoDbIntegrationTests<Startup> dbTestFixture)
        {
            _dynamoDb = dbTestFixture.DynamoDbContext;
            _logger = new Mock<ILogger<PatchesGateway>>();
            _classUnderTest = new PatchesGateway(_dynamoDb, _logger.Object);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                foreach (var action in _cleanup)
                    action();

                _disposed = true;
            }
        }

        private PatchesQueryObject ConstructQuery(Guid id)
        {
            return new PatchesQueryObject() { Id = id };
        }

        [Fact]

        public async Task GetPatchByIdReturnsNullIfEntityDoesntExist()
        {
            var entity = _fixture.Build<PatchEntity>()
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
            var query = ConstructQuery(entity.Id);
            var response = await _classUnderTest.GetPatchByIdAsync(query).ConfigureAwait(false);

            response.Should().BeNull();
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id parameter {query.Id}", Times.Once());

        }

        [Fact]
        public async Task GetPatchByIdReturnsThePatchIfItExists()
        {
            var entity = _fixture.Build<PatchEntity>()
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
            var dbEntity = entity.ToDatabase();

            await InsertDatatoDynamoDB(dbEntity).ConfigureAwait(false);

            var query = ConstructQuery(entity.Id);

            var result = await _classUnderTest.GetPatchByIdAsync(query).ConfigureAwait(false);
            result.Should().BeEquivalentTo(dbEntity, config => config.Excluding(y => y.VersionNumber));
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id parameter {query.Id}", Times.Once());
        }

        [Fact]
        public async Task GetByParentIdReturnsEmptyIfNoRecords()
        {
            var query = new GetPatchByParentIdQuery() { ParentId = Guid.NewGuid() };
            Thread.Sleep(5000);
            var response = await _classUnderTest.GetByParentIdAsync(query).ConfigureAwait(false);
            response.Should().BeEmpty();

            _logger.VerifyExact(LogLevel.Debug, $"Querying PatchByParentId index for parentId {query.ParentId}", Times.Once());
        }

        [Fact]
        public async Task GetByParentIdReturnsRecords()
        {
            var parentid = Guid.NewGuid();
            var patches = new List<PatchesDb>();

            patches.AddRange(_fixture.Build<PatchesDb>()
                                  .With(x => x.ParentId, parentid)
                                  .With(x => x.VersionNumber, (int?) null)

                                  .CreateMany(5));
            InsertListDatatoDynamoDB(patches);

            var query = new GetPatchByParentIdQuery() { ParentId = parentid };
            var response = await _classUnderTest.GetByParentIdAsync(query).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(patches);

            _logger.VerifyExact(LogLevel.Debug, $"Querying PatchByParentId index for parentId {query.ParentId}", Times.Once());
        }

        private async Task InsertDatatoDynamoDB(PatchesDb dbEntity)
        {
            await _dynamoDb.SaveAsync<PatchesDb>(dbEntity).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(dbEntity).ConfigureAwait(false));
        }

        private void InsertListDatatoDynamoDB(List<PatchesDb> dbEntity)
        {
            foreach (var patch in dbEntity)
            {
                _dynamoDb.SaveAsync(patch).GetAwaiter().GetResult();
                _cleanup.Add(async () => await _dynamoDb.DeleteAsync(patch).ConfigureAwait(false));

            }
        }


    }
}
