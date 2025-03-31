using AutoFixture;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using Moq;
using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Xunit;
using FluentAssertions;
using PatchesAndAreasApi.V1;

namespace PatchesAndAreasApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetByPatchNameUseCaseTests
    {
        private Mock<IPatchesGateway> _mockGateway;
        private GetByPatchNameUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();


        public GetByPatchNameUseCaseTests()
        {
            _mockGateway = new Mock<IPatchesGateway>();
            _classUnderTest = new GetByPatchNameUseCase(_mockGateway.Object);
        }

        private GetByPatchNameQuery ConstructQueryParameter()
        {
            return _fixture.Create<GetByPatchNameQuery>();
        }



        [Fact]
        public async Task GetPatchByIdUseCaseGatewayReturnsNullReturnsNull()
        {
            // Arrange
            var query = ConstructQueryParameter();
            _mockGateway.Setup(x => x.GetByPatchNameAsync(query)).ReturnsAsync((PatchEntity) null);

            // Act
            var response = await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetByPatchNameAsyncFoundReturnsResponse()
        {
            // Arrange
            var query = ConstructQueryParameter();
            var patch = _fixture.Create<PatchEntity>();
            _mockGateway.Setup(x => x.GetByPatchNameAsync(query)).ReturnsAsync(patch);

            // Act
            var response = await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(patch);
        }

        [Fact]
        public async Task GetByPatchNameAsyncExceptionIsThrown()
        {
            // Arrange
            var query = ConstructQueryParameter();
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.GetByPatchNameAsync(query)).ThrowsAsync(exception);

            // Act
            Func<Task<PatchEntity>> func = async () => await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            (await func.Should().ThrowAsync<ApplicationException>()).WithMessage(exception.Message);
        }
    }
}
