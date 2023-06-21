using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase;
using Moq;
using Xunit;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using AutoFixture;
using Hackney.Shared.PatchesAndAreas.Domain;
using System.Collections.Generic;

namespace PatchesAndAreasApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class GetAllPatchesUseCaseTests
    {
        private Mock<IPatchesGateway> _mockGateway;
        private GetAllPatchesUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();


        public GetAllPatchesUseCaseTests()
        {
            _mockGateway = new Mock<IPatchesGateway>();
            _classUnderTest = new GetAllPatchesUseCase(_mockGateway.Object);
        }

        [Fact]
        public async Task GetAllPatchesUseCaseReturnsReturnsAllPatches()
        {
            // Arrange
            var patchesResponse = _fixture.Create<List<PatchEntity>>();
            _mockGateway.Setup(x => x.GetAllPatchesAsync()).ReturnsAsync(patchesResponse);

            // Act
            var response = await _classUnderTest.Execute().ConfigureAwait(false);

            // Assert
            Assert.Equal(patchesResponse, response);
        }

        [Fact]
        public async Task GetAllPatchesUseCaseThrowsException()
        {
            // Arrange
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.GetAllPatchesAsync()).ThrowsAsync(exception);

            // Act
            Func<Task<List<PatchEntity>>> func = async () => await _classUnderTest.Execute().ConfigureAwait(false);

            // Assert
            (await func.Should().ThrowAsync<ApplicationException>()).WithMessage(exception.Message);
        }
    }
}
