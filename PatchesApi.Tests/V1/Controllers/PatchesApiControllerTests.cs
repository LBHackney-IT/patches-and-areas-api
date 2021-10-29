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
using System.Collections.Generic;

namespace PatchesApi.Tests.V1.Controllers
{
    [Collection("LogCall collection")]
    public class PatchesApiControllerTests
    {
        private PatchesApiController _classUnderTest;
        private Mock<IGetPatchByIdUseCase> _mockGetByIdUseCase;
        private Mock<IGetPatchByParentIdUseCase> _mockGetByParentIdUseCase;
        private readonly Fixture _fixture = new Fixture();


        public PatchesApiControllerTests()
        {
            var stubHttpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext(new ActionContext(stubHttpContext, new RouteData(), new ControllerActionDescriptor()));

            _mockGetByIdUseCase = new Mock<IGetPatchByIdUseCase>();
            _mockGetByParentIdUseCase = new Mock<IGetPatchByParentIdUseCase>();
            _classUnderTest = new PatchesApiController(_mockGetByIdUseCase.Object, _mockGetByParentIdUseCase.Object);
            _classUnderTest.ControllerContext = controllerContext;

        }

        private PatchesQueryObject ConstructQuery()
        {
            return new PatchesQueryObject() { Id = Guid.NewGuid() };
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
    }
}
