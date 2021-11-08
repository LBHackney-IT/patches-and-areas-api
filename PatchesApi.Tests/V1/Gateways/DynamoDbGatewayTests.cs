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
using System.Linq;
using PatchesApi.V1.Infrastructure.Exceptions;

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

        private UpdatePatchesResponsibilityRequest ConstructUpdateQuery(Guid id, Guid responsibilityId)
        {
            return new UpdatePatchesResponsibilityRequest() { Id = id, ResponsibileEntityId = responsibilityId };
        }
        private UpdatePatchesResponsibilitiesRequestObject ConstructUpdateRequest(Guid responsibilityId)
        {
            var request = _fixture.Build<UpdatePatchesResponsibilitiesRequestObject>()
                                  .With(x => x.Id, responsibilityId).Create();
            return request;
        }





        [Fact]

        public async Task GetPatchByIdReturnsNullIfEntityDoesntExist()
        {
            //Arrange
            var entity = _fixture.Build<PatchEntity>()
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
            var query = ConstructQuery(entity.Id);
            //Act
            var response = await _classUnderTest.GetPatchByIdAsync(query).ConfigureAwait(false);

            //Assert
            response.Should().BeNull();
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id parameter {query.Id}", Times.Once());

        }

        [Fact]
        public async Task GetPatchByIdReturnsThePatchIfItExists()
        {
            //Arrange
            var entity = _fixture.Build<PatchEntity>()
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
            var dbEntity = entity.ToDatabase();

            await InsertDatatoDynamoDB(dbEntity).ConfigureAwait(false);

            var query = ConstructQuery(entity.Id);
            //Act
            var result = await _classUnderTest.GetPatchByIdAsync(query).ConfigureAwait(false);

            //Assert
            result.Should().BeEquivalentTo(dbEntity, config => config.Excluding(y => y.VersionNumber));
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id parameter {query.Id}", Times.Once());
        }

        [Fact]
        public async Task UpdatePatchWithNewResponsibileEntitySuccessfullyUpdates()
        {
            //Arrange
            var entity = _fixture.Build<PatchEntity>()
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
            var dbEntity = entity.ToDatabase();
            await InsertDatatoDynamoDB(dbEntity).ConfigureAwait(false);


            var query = ConstructUpdateQuery(entity.Id, Guid.NewGuid());
            var request = ConstructUpdateRequest(query.ResponsibileEntityId);
            dbEntity.VersionNumber = 0;

            //Act
            var result = await _classUnderTest.UpdatePatchResponsibilities(query, request, 0).ConfigureAwait(false);

            //Assert
            var load = await _dynamoDb.LoadAsync<PatchesDb>(dbEntity.Id).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync<PatchesDb>(load.Id).ConfigureAwait(false));

            //Updated tenure with new Household Member
            result.Should().BeEquivalentTo(load, config => config.Excluding(y => y.VersionNumber));


            load.VersionNumber.Should().Be(1);

            var expected = new ResponsibleEntities()
            {
                Id = query.ResponsibileEntityId,
                Name = request.Name,
                ResponsibleType = request.ResponsibleType

            };
            load.ResponsibleEntities.Should().ContainEquivalentOf(expected);
        }



        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public async Task UpdateTenureForPersonThrowsExceptionOnVersionConflict(int? ifMatch)
        {
            // Arrange
            var entity = _fixture.Build<PatchEntity>()
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();

            var query = ConstructUpdateQuery(entity.Id, entity.ResponsibleEntities.First().Id);
            var dbEntity = entity.ToDatabase();

            await InsertDatatoDynamoDB(dbEntity).ConfigureAwait(false);
            entity.VersionNumber = 0;

            var constructRequest = ConstructUpdateRequest(query.ResponsibileEntityId);

            //Act
            Func<Task<PatchesDb>> func = async () => await _classUnderTest.UpdatePatchResponsibilities(query, constructRequest, ifMatch)
                                                                                                   .ConfigureAwait(false);

            // Assert
            func.Should().Throw<VersionNumberConflictException>()
                         .Where(x => (x.IncomingVersionNumber == ifMatch) && (x.ExpectedVersionNumber == 0));
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync to update id {query.Id}", Times.Never());
        }

        private async Task InsertDatatoDynamoDB(PatchesDb dbEntity)
        {
            await _dynamoDb.SaveAsync<PatchesDb>(dbEntity).ConfigureAwait(false);
            _cleanup.Add(async () => await _dynamoDb.DeleteAsync(dbEntity).ConfigureAwait(false));
        }
    }
}
