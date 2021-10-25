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
            _logger = new Mock<ILogger<PatchesGateway>>();
            _dynamoDb = dbTestFixture.DynamoDbContext;
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

        [Fact(Skip = "Enable if using DynamoDb")]

        public async Task GetEntityByIdReturnsNullIfEntityDoesntExist()
        {
            var entity = _fixture.Build<Entity>()
                                   .With(x => x.CreatedAt, DateTime.UtcNow).Create();
            var query = ConstructQuery(entity.Id);
            var response = await _classUnderTest.GetEntityById(query).ConfigureAwait(false);

            response.Should().BeNull();
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id parameter 123", Times.Once());

        }

        [Fact(Skip = "Enable if using DynamoDb")]
        public async Task VerifiesGatewayMethodsAddtoDB()
        {
            var entity = _fixture.Build<Entity>()
                                   .With(x => x.CreatedAt, DateTime.UtcNow).Create();
            await InsertDatatoDynamoDB(entity).ConfigureAwait(false);

            var query = ConstructQuery(entity.Id);

            var result = await _classUnderTest.GetEntityById(query).ConfigureAwait(false);
            result.Should().BeEquivalentTo(entity);
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id parameter {entity.Id}", Times.Once());
        }

        private async Task InsertDatatoDynamoDB(Entity entity)
        {
            await _dynamoDb.SaveAsync<Entity>(entity).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(entity).ConfigureAwait(false));
        }
    }
}
