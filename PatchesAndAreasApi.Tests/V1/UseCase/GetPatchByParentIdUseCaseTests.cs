using AutoFixture;
using FluentAssertions;
using Moq;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PatchesAndAreasApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetPatchByParentIdUseCaseTests
    {
        private Mock<IPatchesGateway> _mockGateway;
        private GetPatchByParentIdUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();


        public GetPatchByParentIdUseCaseTests()
        {
            _mockGateway = new Mock<IPatchesGateway>();
            _classUnderTest = new GetPatchByParentIdUseCase(_mockGateway.Object);
        }

        private GetPatchByParentIdQuery ConstructQueryParameter()
        {
            return new GetPatchByParentIdQuery() { ParentId = Guid.NewGuid() };
        }



        [Fact]
        public async Task GetPatchByIdUseCaseGatewayReturnsNullReturnsNull()
        {
            // Arrange
            var query = ConstructQueryParameter();
            _mockGateway.Setup(x => x.GetByParentIdAsync(query)).ReturnsAsync((List<PatchEntity>) null);

            // Act
            var response = await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task GetPatchByIdAsyncFoundReturnsResponse()
        {
            // Arrange
            var query = ConstructQueryParameter();
            var patch = _fixture.Create<List<PatchEntity>>();
            _mockGateway.Setup(x => x.GetByParentIdAsync(query)).ReturnsAsync(patch);

            // Act
            var response = await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            response.Should().BeEquivalentTo(patch);
        }

        [Fact]
        public async Task GetPatchByIdAsyncExceptionIsThrown()
        {
            // Arrange
            var query = ConstructQueryParameter();
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.GetByParentIdAsync(query)).ThrowsAsync(exception);

            // Act
            Func<Task<List<PatchEntity>>> func = async () => await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            (await func.Should().ThrowAsync<ApplicationException>()).WithMessage(exception.Message);
        }
    }
}
