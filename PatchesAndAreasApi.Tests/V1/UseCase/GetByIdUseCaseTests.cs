using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase;
using Moq;
using Xunit;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using AutoFixture;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;

namespace PatchesAndAreasApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetByIdUseCaseTests
    {
        private Mock<IPatchesGateway> _mockGateway;
        private GetPatchByIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();


        public GetByIdUseCaseTests()
        {
            _mockGateway = new Mock<IPatchesGateway>();
            _classUnderTest = new GetPatchByIdUseCase(_mockGateway.Object);
        }

        private PatchesQueryObject ConstructQuery()
        {
            return new PatchesQueryObject() { Id = Guid.NewGuid() };
        }

        [Fact]
        public async Task GetPatchByIdUseCaseGatewayReturnsNullReturnsNull()
        {
            // Arrange
            var query = ConstructQuery();
            _mockGateway.Setup(x => x.GetPatchByIdAsync(query)).ReturnsAsync((PatchEntity) null);

            // Act
            var response = await _classUnderTest.Execute(query).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetPatchByIdAsyncFoundReturnsResponse()
        {
            // Arrange
            var query = ConstructQuery();
            var person = _fixture.Create<PatchEntity>();
            _mockGateway.Setup(x => x.GetPatchByIdAsync(query)).ReturnsAsync(person);

            // Act
            var response = await _classUnderTest.Execute(query).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(person);
        }

        [Fact]
        public async Task GetPatchByIdAsyncExceptionIsThrown()
        {
            // Arrange
            var query = ConstructQuery();
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.GetPatchByIdAsync(query)).ThrowsAsync(exception);

            // Act
            Func<Task<PatchEntity>> func = async () => await _classUnderTest.Execute(query).ConfigureAwait(false);

            // Assert
            (await func.Should().ThrowAsync<ApplicationException>()).WithMessage(exception.Message);
        }

    }
}
