using AutoFixture;
using FluentAssertions;
using Moq;
using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Domain;
using PatchesApi.V1.Gateways;
using PatchesApi.V1.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PatchesApi.Tests.V1.UseCase
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
        public void GetPatchByIdAsyncExceptionIsThrown()
        {
            // Arrange
            var query = ConstructQueryParameter();
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.GetByParentIdAsync(query)).ThrowsAsync(exception);

            // Act
            Func<Task<List<PatchEntity>>> func = async () => await _classUnderTest.ExecuteAsync(query).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }
}