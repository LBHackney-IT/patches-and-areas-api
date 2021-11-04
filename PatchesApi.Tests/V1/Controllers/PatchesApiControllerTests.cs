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
using PatchesApi.V1.Infrastructure;
using Microsoft.Extensions.Primitives;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using PatchesApi.V1.Infrastructure.Exceptions;

namespace PatchesApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class PatchesApiControllerTests
    {
        private PatchesApiController _classUnderTest;
        private Mock<IGetPatchByIdUseCase> _mockGetByIdUseCase;
        private Mock<IDeleteResponsibilityFromPatchUseCase> _mockDeleteResponsibilityFromPatchUseCase;
        private readonly Fixture _fixture = new Fixture();


        public PatchesApiControllerTests()
        {
            var stubHttpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext(new ActionContext(stubHttpContext, new RouteData(), new ControllerActionDescriptor()));

            _mockGetByIdUseCase = new Mock<IGetPatchByIdUseCase>();
            _mockDeleteResponsibilityFromPatchUseCase = new Mock<IDeleteResponsibilityFromPatchUseCase>();
            _classUnderTest = new PatchesApiController(_mockGetByIdUseCase.Object, _mockDeleteResponsibilityFromPatchUseCase.Object);
            _classUnderTest.ControllerContext = controllerContext;

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
            _classUnderTest.HttpContext.Response.Headers.TryGetValue(HeaderConstants.ETag, out StringValues val).Should().BeTrue();
            val.First().Should().Be($"\"{patchResponse.VersionNumber.ToString()}\"");
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

        [Fact]
        public async Task DeleteResponsibilityFromPatchDoesntExistReturnsNotFound()
        {
            // Arrange
            var mockQuery = _fixture.Create<DeleteResponsibilityFromPatchRequest>();

            var exception = new PatchNotFoundException();

            _mockDeleteResponsibilityFromPatchUseCase
                .Setup(x => x.Execute(It.IsAny<DeleteResponsibilityFromPatchRequest>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _classUnderTest.DeleteResponsibilityFromPatch(mockQuery).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(NotFoundObjectResult));
            (result as NotFoundObjectResult).Value.Should().Be(mockQuery.Id);
        }

        [Fact]
        public async Task DeleteResponsibilityFromPatcheWhenResponsibilityDoesntExistInPatchReturnsNotFound()
        {
            // Arrange
            var mockQuery = _fixture.Create<DeleteResponsibilityFromPatchRequest>();

            var exception = new ResponsibileIdNotFoundInPatchException();

            _mockDeleteResponsibilityFromPatchUseCase
                .Setup(x => x.Execute(It.IsAny<DeleteResponsibilityFromPatchRequest>()))
                .ThrowsAsync(exception);

            // Act
            var result = await _classUnderTest.DeleteResponsibilityFromPatch(mockQuery).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(NotFoundObjectResult));
            (result as NotFoundObjectResult).Value.Should().Be(mockQuery.ResponsibileEntityId);
        }

        [Fact]
        public async Task DeleteResponsibilityFromPatchWhenResponsibilityExistsReturnsNoContentResponseAsync()
        {
            // Arrange
            var mockQuery = _fixture.Create<DeleteResponsibilityFromPatchRequest>();

            // Act
            var result = await _classUnderTest.DeleteResponsibilityFromPatch(mockQuery).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(NoContentResult));
        }

    }
}
