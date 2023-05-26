using AutoFixture;
using FluentAssertions;
using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using Hackney.Shared.PatchesAndAreas.Factories;
using Hackney.Shared.PatchesAndAreas.Infrastructure;
using Hackney.Shared.PatchesAndAreas.Infrastructure.Exceptions;
using PatchesAndAreasApi.V1.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.FileSystemGlobbing;

namespace PatchesAndAreasApi.Tests.V1.Gateways
{
    [Collection("AppTest collection")]
    public class DynamoDbGatewayTests : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly IDynamoDbFixture _dbFixture;
        private readonly PatchesGateway _classUnderTest;
        private readonly Random _random = new Random();

        private readonly Mock<ILogger<PatchesGateway>> _logger;

        public DynamoDbGatewayTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _logger = new Mock<ILogger<PatchesGateway>>();
            _classUnderTest = new PatchesGateway(_dbFixture.DynamoDbContext, _logger.Object);
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

            await InsertDataToDynamoDB(dbEntity).ConfigureAwait(false);

            var query = ConstructQuery(entity.Id);
            //Act
            var result = await _classUnderTest.GetPatchByIdAsync(query).ConfigureAwait(false);

            //Assert
            result.Should().BeEquivalentTo(dbEntity, config => config.Excluding(y => y.VersionNumber));
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.LoadAsync for id parameter {query.Id}", Times.Once());
        }

        [Fact]
        public async Task DeleteResponsibilityFromPatchWhenPatchDoesntExistThrowsException()
        {
            // Arrange
            var mockRequest = _fixture.Create<DeleteResponsibilityFromPatchRequest>();

            // Act
            Func<Task> func = async () => await _classUnderTest.DeleteResponsibilityFromPatch(mockRequest).ConfigureAwait(false);

            // Assert
            await func.Should().ThrowAsync<PatchNotFoundException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteResponsibilityFromPatchWhenResponsibilityNotInPatchThrowsException()
        {
            // Arrange
            var mockPatch = _fixture.Build<PatchEntity>()
                .With(x => x.ResponsibleEntities, new List<ResponsibleEntities>()) // empty list
                .With(x => x.VersionNumber, (int?) null)
                .Create();
            var dbPatch = mockPatch.ToDatabase();

            await InsertDataToDynamoDB(dbPatch).ConfigureAwait(false);

            var mockRequest = new DeleteResponsibilityFromPatchRequest
            {
                Id = mockPatch.Id,
                ResponsibileEntityId = _fixture.Create<Guid>()
            };

            // Act
            Func<Task> func = async () => await _classUnderTest.DeleteResponsibilityFromPatch(mockRequest).ConfigureAwait(false);

            // Assert
            await func.Should().ThrowAsync<ResponsibileIdNotFoundInPatchException>().ConfigureAwait(false);
        }

        [Fact]
        public async Task DeleteResponsibilityFromPatchWhenCalledRemovesSuccessfully()
        {
            // Arrange
            var numberOfResponsibilities = _random.Next(2, 5);
            var mockResponsibileEntity = _fixture.Build<ResponsibleEntities>().CreateMany(numberOfResponsibilities);

            var mockPatch = _fixture.Build<PatchEntity>()
                .With(x => x.ResponsibleEntities, mockResponsibileEntity)
                .With(x => x.VersionNumber, (int?) null)
                .Create();

            var responsibilityToRemove = mockResponsibileEntity.First();
            var dbEntity = mockPatch.ToDatabase();
            await InsertDataToDynamoDB(dbEntity).ConfigureAwait(false);

            var mockRequest = new DeleteResponsibilityFromPatchRequest
            {
                Id = mockPatch.Id,
                ResponsibileEntityId = responsibilityToRemove.Id
            };

            // Act
            Func<Task> func = async () => await _classUnderTest.DeleteResponsibilityFromPatch(mockRequest).ConfigureAwait(false);

            // Assert
            // no exception of any kind thrown
            await func.Should().NotThrowAsync<Exception>().ConfigureAwait(false);

            // check database
            var databaseResponse = await _dbFixture.DynamoDbContext.LoadAsync<PatchesDb>(mockPatch.Id).ConfigureAwait(false);
            databaseResponse.ResponsibleEntities.Should().HaveCount(numberOfResponsibilities - 1);

            databaseResponse.ResponsibleEntities.Should().NotContain(x => x.Id == responsibilityToRemove.Id);
        }
        [Fact]
        public async Task UpdatePatchWithNewResponsibileEntitySuccessfullyUpdates()
        {
            //Arrange
            var entity = _fixture.Build<PatchEntity>()
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();
            var dbEntity = entity.ToDatabase();
            await InsertDataToDynamoDB(dbEntity).ConfigureAwait(false);


            var query = ConstructUpdateQuery(entity.Id, Guid.NewGuid());
            var request = ConstructUpdateRequest(query.ResponsibileEntityId);
            dbEntity.VersionNumber = 0;

            //Act
            var result = await _classUnderTest.UpdatePatchResponsibilities(query, request, 0).ConfigureAwait(false);

            //Assert
            var load = await _dbFixture.DynamoDbContext.LoadAsync<PatchesDb>(dbEntity.Id).ConfigureAwait(false);

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
        public async Task UpdatePatchWithNewResponsibilityEntityThrowsExceptionOnVersionConflict(int? ifMatch)
        {
            // Arrange
            var entity = _fixture.Build<PatchEntity>()
                                 .With(x => x.VersionNumber, (int?) null)
                                 .Create();

            var query = ConstructUpdateQuery(entity.Id, entity.ResponsibleEntities.First().Id);
            var dbEntity = entity.ToDatabase();

            await InsertDataToDynamoDB(dbEntity).ConfigureAwait(false);
            entity.VersionNumber = 0;

            var constructRequest = ConstructUpdateRequest(query.ResponsibileEntityId);

            //Act
            Func<Task<PatchesDb>> func = async () => await _classUnderTest.UpdatePatchResponsibilities(query, constructRequest, ifMatch)
                                                                                                   .ConfigureAwait(false);

            // Assert
            (await func.Should().ThrowAsync<VersionNumberConflictException>())
                         .Where(x => (x.IncomingVersionNumber == ifMatch) && (x.ExpectedVersionNumber == 0));
            _logger.VerifyExact(LogLevel.Debug, $"Calling IDynamoDBContext.SaveAsync to update id {query.Id}", Times.Never());
        }

        [Fact]
        public async Task GetByParentIdReturnsEmptyIfNoRecords()
        {
            var query = new GetPatchByParentIdQuery() { ParentId = Guid.NewGuid() };
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
            InsertListDataToDynamoDB(patches);

            var query = new GetPatchByParentIdQuery() { ParentId = parentid };
            var response = await _classUnderTest.GetByParentIdAsync(query).ConfigureAwait(false);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(patches);

            _logger.VerifyExact(LogLevel.Debug, $"Querying PatchByParentId index for parentId {query.ParentId}", Times.Once());
        }

        //[Fact]
        //public async Task GetAllPatchesAsyncReturnsEmptyListIfNoPatchesExist()
        //{
        //    _dbFixture.DynamoDbContext.Dispose();
        //    // Act
        //    var result = await _classUnderTest.GetAllPatchesAsync().ConfigureAwait(false);

        //    // Assert
        //    result.Should().BeEmpty();
        //    _logger.VerifyExact(LogLevel.Debug, "Calling IDynamoDBContext.ScanAsync for all PatchEntity records", Times.Once());
        //}

        [Fact]
        public async Task GetAllPatchesAsyncReturnsAllPatchesIfTheyExist()
        {
            // Arrange
            //_dbFixture.DynamoDbContext.Dispose();

            var patches = _fixture.Build<PatchesDb>()
                                  .With(x => x.VersionNumber, (int?) null)
                                  .Without(x => x.ResponsibleEntities)
                                  .CreateMany(5).ToList();

            InsertListDataToDynamoDB(patches);

            // Act
            var result = await _classUnderTest.GetAllPatchesAsync().ConfigureAwait(false);

            // Assert
            result.Should().BeEquivalentTo(patches);
            _logger.VerifyExact(LogLevel.Debug, "Calling IDynamoDBContext.ScanAsync for all PatchEntity records", Times.Once());
        }


        private async Task InsertDataToDynamoDB(PatchesDb dbEntity)
        {
            await _dbFixture.SaveEntityAsync(dbEntity).ConfigureAwait(false);
        }

        private void InsertListDataToDynamoDB(List<PatchesDb> dbEntity)
        {
            foreach (var patch in dbEntity)
            {
                _dbFixture.SaveEntityAsync(patch).GetAwaiter().GetResult();
            }
        }
    }
}
