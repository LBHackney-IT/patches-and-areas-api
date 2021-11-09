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
using Hackney.Core.Http;
using PatchesApi.V1.Boundary.Response;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PatchesApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class PatchesApiControllerTests
    {
        private Mock<IGetPatchByIdUseCase> _mockGetByIdUseCase;
        private Mock<IDeleteResponsibilityFromPatchUseCase> _mockDeleteResponsibilityFromPatchUseCase;
        private Mock<IUpdatePatchResponsibilitiesUseCase> _mockPatchResponsibilitiesUseCase;

        private readonly Mock<IHttpContextWrapper> _mockContextWrapper;
        private readonly Mock<HttpRequest> _mockHttpRequest;
        private readonly HeaderDictionary _requestHeaders;
        private readonly Mock<HttpResponse> _mockHttpResponse;

        private PatchesApiController _classUnderTest;

        private Mock<IGetPatchByParentIdUseCase> _mockGetByParentIdUseCase;
        private readonly Fixture _fixture = new Fixture();


        public PatchesApiControllerTests()
        {
            _mockGetByIdUseCase = new Mock<IGetPatchByIdUseCase>();
            _mockDeleteResponsibilityFromPatchUseCase = new Mock<IDeleteResponsibilityFromPatchUseCase>();
            _mockGetByParentIdUseCase = new Mock<IGetPatchByParentIdUseCase>();
            _mockPatchResponsibilitiesUseCase = new Mock<IUpdatePatchResponsibilitiesUseCase>();

            _mockContextWrapper = new Mock<IHttpContextWrapper>();
            _mockHttpRequest = new Mock<HttpRequest>();
            _mockHttpResponse = new Mock<HttpResponse>();

            _classUnderTest = new PatchesApiController(
                _mockGetByIdUseCase.Object,
                _mockPatchResponsibilitiesUseCase.Object,
                _mockGetByParentIdUseCase.Object,
                _mockDeleteResponsibilityFromPatchUseCase.Object,
                _mockContextWrapper.Object);

            _requestHeaders = new HeaderDictionary();
            _mockHttpRequest.SetupGet(x => x.Headers).Returns(_requestHeaders);

            _mockContextWrapper
                .Setup(x => x.GetContextRequestHeaders(It.IsAny<HttpContext>()))
                .Returns(_requestHeaders);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(x => x.Request).Returns(_mockHttpRequest.Object);
            mockHttpContext.SetupGet(x => x.Response).Returns(_mockHttpResponse.Object);


            var controllerContext = new ControllerContext(new ActionContext(mockHttpContext.Object, new RouteData(), new ControllerActionDescriptor()));
            _classUnderTest.ControllerContext = controllerContext;

        }

        private PatchesQueryObject ConstructQuery()
        {
            return new PatchesQueryObject() { Id = Guid.NewGuid() };
        }

        private UpdatePatchesResponsibilityRequest ConstructUpdateQuery()
        {
            return new UpdatePatchesResponsibilityRequest() { Id = Guid.NewGuid(), ResponsibileEntityId = Guid.NewGuid() };
        }

        private UpdatePatchesResponsibilitiesRequestObject ConstructUpdateRequest()
        {
            var request = _fixture.Create<UpdatePatchesResponsibilitiesRequestObject>();

            return request;
        }
        private GetPatchByParentIdQuery ConstructQueryParameter()
        {
            return new GetPatchByParentIdQuery() { ParentId = Guid.NewGuid() };
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
            var stubHttpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext(new ActionContext(stubHttpContext, new RouteData(), new ControllerActionDescriptor()));
            _classUnderTest.ControllerContext = controllerContext;
            // Arrange
            var query = ConstructQuery();
            var patchResponse = _fixture.Create<PatchEntity>();
            _mockGetByIdUseCase.Setup(x => x.Execute(query)).ReturnsAsync(patchResponse);

            // Act
            var response = await _classUnderTest.GetPatchById(query).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(patchResponse.ToResponse());

            var expectedEtagValue = $"\"{patchResponse.VersionNumber}\"";
            _classUnderTest.HttpContext.Response.Headers.TryGetValue(HeaderConstants.ETag, out StringValues val).Should().BeTrue();
            val.First().Should().Be(expectedEtagValue);
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

        [Fact]
        public async Task UpdatePatchByResponsibilityAsyncFoundReturnsFound()
        {
            // Arrange
            var query = ConstructUpdateQuery();
            var request = ConstructUpdateRequest();
            var patchResponse = _fixture.Create<PatchesResponseObject>();
            _mockPatchResponsibilitiesUseCase.Setup(x => x.ExecuteAsync(query, request, It.IsAny<int?>()))
                                    .ReturnsAsync(patchResponse);

            // Act
            var response = await _classUnderTest.UpdatePatchForResponsibility(query, request).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(NoContentResult));
        }

        [Fact]
        public async Task UpdatePatchByResponsibilityAsyncNotFoundReturnsNotFound()
        {
            // Arrange
            var query = ConstructUpdateQuery();
            var request = ConstructUpdateRequest();
            _mockPatchResponsibilitiesUseCase.Setup(x => x.ExecuteAsync(query, request, It.IsAny<int?>()))
                                    .ReturnsAsync((PatchesResponseObject) null);

            // Act
            var response = await _classUnderTest.UpdatePatchForResponsibility(query, request).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(query.Id);
        }

        [Fact]
        public async Task GetPatchByParentIdNotFoundReturnsNotFound()
        {
            var queryParam = ConstructQueryParameter();
            _mockGetByParentIdUseCase.Setup(x => x.ExecuteAsync(queryParam)).ReturnsAsync((List<PatchEntity>) null);

            // Act
            var response = await _classUnderTest.GetByParentIdAsync(queryParam).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(NotFoundObjectResult));
            (response as NotFoundObjectResult).Value.Should().Be(queryParam.ParentId);
        }

        [Fact]
        public async Task GetPatchByParentIdFoundReturnsResponse()
        {
            // Arrange
            var queryParam = ConstructQueryParameter();
            var patchResponseList = _fixture.Create<List<PatchEntity>>();
            _mockGetByParentIdUseCase.Setup(x => x.ExecuteAsync(queryParam)).ReturnsAsync(patchResponseList);

            // Act
            var response = await _classUnderTest.GetByParentIdAsync(queryParam).ConfigureAwait(false);

            // Assert
            response.Should().BeOfType(typeof(OkObjectResult));
            (response as OkObjectResult).Value.Should().BeEquivalentTo(patchResponseList.ToResponse());
        }

        [Fact]
        public void GetPatchByParentIdExceptionIsThrown()
        {
            // Arrange
            var queryParam = ConstructQueryParameter();
            var exception = new ApplicationException("Test exception");
            _mockGetByParentIdUseCase.Setup(x => x.ExecuteAsync(queryParam)).ThrowsAsync(exception);

            // Act
            Func<Task<IActionResult>> func = async () => await _classUnderTest.GetByParentIdAsync(queryParam).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData(0, 1)]
        [InlineData(0, null)]
        [InlineData(2, 1)]
        public async Task UpdatePersonByIdAsyncVersionNumberConflictExceptionReturns409(int? expected, int? actual)
        {
            // Arrange
            var query = ConstructUpdateQuery();

            _requestHeaders.Add(HeaderConstants.IfMatch, $"\"{expected?.ToString()}\"");

            var exception = new VersionNumberConflictException(expected, actual);
            _mockPatchResponsibilitiesUseCase.Setup(x => x.ExecuteAsync(query, It.IsAny<UpdatePatchesResponsibilitiesRequestObject>(), expected))
                                    .ThrowsAsync(exception);

            // Act
            var result = await _classUnderTest.UpdatePatchForResponsibility(query, new UpdatePatchesResponsibilitiesRequestObject()).ConfigureAwait(false);

            // Assert
            result.Should().BeOfType(typeof(ConflictObjectResult));
            (result as ConflictObjectResult).Value.Should().BeEquivalentTo(exception.Message);
        }
    }
}
