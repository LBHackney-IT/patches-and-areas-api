using AutoFixture;
using Hackney.Shared.PatchesAndAreas.Boundary.Response;
using Hackney.Shared.PatchesAndAreas.Infrastructure;
using Moq;
using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase;
using System.Threading.Tasks;
using System;
using Xunit;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using System.Collections.Generic;
using Hackney.Shared.PatchesAndAreas.Domain;
using System.Linq;
using FluentAssertions;
using Hackney.Shared.PatchesAndAreas.Factories;
using Hackney.Core.Sns;
using PatchesAndAreasApi.V1.Factories;
using Hackney.Core.JWT;
using PatchesAndAreasApi.V1.Domain;

namespace PatchesAndAreasApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class ReplacePatchResponsibleEntitiesUseCaseTests
    {
        private readonly Mock<IPatchesGateway> _mockGateway;
        private readonly Mock<ISnsGateway> _mockSnsGateway;
        private readonly Mock<ISnsFactory> _mockSnsFactory;

        private readonly ReplacePatchResponsibleEntitiesUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public ReplacePatchResponsibleEntitiesUseCaseTests()
        {
            _mockGateway = new Mock<IPatchesGateway>();
            _mockSnsGateway = new Mock<ISnsGateway>();
            _mockSnsFactory = new Mock<ISnsFactory>();
            _classUnderTest = new ReplacePatchResponsibleEntitiesUseCase(_mockGateway.Object, _mockSnsGateway.Object, _mockSnsFactory.Object);
        }

        private PatchesQueryObject ConstructQuery()
        {
            return new PatchesQueryObject() { Id = Guid.NewGuid() };
        }

        private List<ResponsibleEntities> ConstructRequest()
        {
            var listOfResponsibleEntitites = _fixture.Build<ResponsibleEntities>().CreateMany(2).ToList();
            return listOfResponsibleEntitites;
        }

        private PatchesDb ConstructUpdateResponse(Guid id)
        {
            return _fixture.Build<PatchesDb>()
                            .With(y => y.Id, id)
                            .Create();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(3)]
        public async Task ReplacePatchResponsibleEntitiesUseCaseReturnsResult(int? ifMatch)
        {
            //Arrange
            var token = new Token();
            var request = ConstructRequest();
            var query = ConstructQuery();
            var gatewayResponse = ConstructUpdateResponse(query.Id);

            _mockGateway.Setup(x => x.ReplacePatchResponsibleEntities(query, request, ifMatch)).ReturnsAsync(gatewayResponse);
            var snsEvent = _fixture.Create<PatchesAndAreasSns>();
            _mockSnsFactory.Setup(x => x.Update(gatewayResponse, token, It.IsAny<ResponsibleEntities>()))
                           .Returns(snsEvent);
            //Act
            var response = await _classUnderTest.ExecuteAsync(query, request, ifMatch, token).ConfigureAwait(false);
            //Assert
            response.Should().BeEquivalentTo(gatewayResponse.ToDomain().ToResponse());
            _mockSnsFactory.Verify(x => x.Update(gatewayResponse, token, It.IsAny<ResponsibleEntities>()), Times.Once);
            _mockSnsGateway.Verify(x => x.Publish(snsEvent, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(3)]
        public async Task ReplacePatchResponsibleEntitiesUseCaseReturnsNull(int? ifMatch)
        {
            //Arrange
            var token = new Token();
            var request = ConstructRequest();
            var query = ConstructQuery();

            _mockGateway.Setup(x => x.ReplacePatchResponsibleEntities(query, request, ifMatch)).ReturnsAsync((PatchesDb) null);
            //Act
            var response = await _classUnderTest.ExecuteAsync(query, request, ifMatch, token).ConfigureAwait(false);

            //Assert
            response.Should().BeNull();
            _mockSnsFactory.Verify(x => x.Update(It.IsAny<PatchesDb>(), token, It.IsAny<ResponsibleEntities>()), Times.Never);
            _mockSnsGateway.Verify(x => x.Publish(It.IsAny<PatchesAndAreasSns>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }

        [Theory]
        [InlineData(null)]
        [InlineData(3)]
        public async Task ReplacePatchResponsibleEntitiesAsyncExceptionIsThrown(int? ifMatch)
        {
            // Arrange
            var token = new Token();
            var request = ConstructRequest();
            var query = ConstructQuery();
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.ReplacePatchResponsibleEntities(query, request, ifMatch)).ThrowsAsync(exception);

            // Act
            Func<Task<PatchesResponseObject>> func = async () =>
                await _classUnderTest.ExecuteAsync(query, request, ifMatch, token).ConfigureAwait(false);

            // Assert
            (await func.Should().ThrowAsync<ApplicationException>()).WithMessage(exception.Message);
        }
    }
}
