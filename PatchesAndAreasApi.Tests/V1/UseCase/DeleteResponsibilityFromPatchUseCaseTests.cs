using AutoFixture;
using Moq;
using PatchesAndAreas.Boundary.Request;
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
    public class DeleteResponsibilityFromPatchUseCaseTests
    {
        private readonly Mock<IPatchesGateway> _mockGateway;

        private readonly DeleteResponsibilityFromPatchUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public DeleteResponsibilityFromPatchUseCaseTests()
        {
            _mockGateway = new Mock<IPatchesGateway>();

            _classUnderTest = new DeleteResponsibilityFromPatchUseCase(_mockGateway.Object);
        }

        [Fact]
        public async Task WhenCalledCallsUseCase()
        {
            // Arrange
            var mockQuery = _fixture.Create<DeleteResponsibilityFromPatchRequest>();

            // Act
            await _classUnderTest.Execute(mockQuery).ConfigureAwait(false);

            // Assert
            _mockGateway.Verify(x => x.DeleteResponsibilityFromPatch(mockQuery));
        }


    }
}
