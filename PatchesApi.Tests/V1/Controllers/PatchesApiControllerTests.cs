using AutoFixture;
using PatchesApi.V1.Controllers;
using PatchesApi.V1.UseCase.Interfaces;
using Moq;
using Xunit;
using PatchesApi.V1.Boundary.Request;
using System;
using System.Threading.Tasks;
using PatchesApi.V1.Domain;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using PatchesApi.V1.Factories;

namespace PatchesApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class PatchesApiControllerTests
    {
        private PatchesApiController _classUnderTest;
        private Mock<IGetPatchByIdUseCase> _mockGetByIdUseCase;
        private readonly Fixture _fixture = new Fixture();


        public PatchesApiControllerTests()
        {
            _mockGetByIdUseCase = new Mock<IGetPatchByIdUseCase>();
            _classUnderTest = new PatchesApiController(_mockGetByIdUseCase.Object);
        }

        private PatchesQueryObject ConstructQuery()
        {
            return new PatchesQueryObject() { Id = Guid.NewGuid() };
        }

        [Fact]
        public async Task GetPatchByIdNotFoundReturnsNotFound()
        {
            // Arrange
            var query = ConstructQuery();
            _mockGetByIdUseCase.Setup(x => x.Execute(query)).ReturnsAsync((PatchEntity) null);

            // Act
            var response = await _classUnderTest.GetPatchById(query).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.Id);
        }

        [Fact]
        public async Task GetPatchByIdFoundReturnsResponse()
        {
            // Arrange
            var query = ConstructQuery();
            var patchResponse = _fixture.Create<PatchEntity>();
            _mockGetByIdUseCase.Setup(x => x.Execute(query)).ReturnsAsync(patchResponse);

            // Act
            var response = await _classUnderTest.GetPatchById(query).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(patchResponse.ToResponse());
        }

        [Fact]
        public void GetPatchByIdExceptionIsThrown()
        {
            // Arrange
            var query = ConstructQuery();
            var exception = new ApplicationException("Test exception");
            _mockGetByIdUseCase.Setup(x => x.Execute(query)).ThrowsAsync(exception);

            // Act
            Func<Task<IActionResult>> func = async () => await _classUnderTest.GetPatchById(query).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }

    }
}
