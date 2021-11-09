using AutoFixture;
using FluentAssertions;
using Moq;
using PatchesAndAreasApi.V1.Boundary.Request;
using PatchesAndAreasApi.V1.Boundary.Response;
using PatchesAndAreasApi.V1.Factories;
using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.Infrastructure;
using PatchesAndAreasApi.V1.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PatchesAndAreasApi.Tests.V1.UseCase
{
    [Collection("LogCall collection")]
    public class UpdatePatchResponsibilitiesUseCaseTests
    {
        private readonly Mock<IPatchesGateway> _mockGateway;
        private readonly UpdatePatchResponsibilitiesUseCase _classUnderTest;
        private readonly Fixture _fixture = new Fixture();

        public UpdatePatchResponsibilitiesUseCaseTests()
        {
            _mockGateway = new Mock<IPatchesGateway>();
            _classUnderTest = new UpdatePatchResponsibilitiesUseCase(_mockGateway.Object);
        }

        private UpdatePatchesResponsibilityRequest ConstructQuery(Guid? id = null, Guid? responsibilityId = null)
        {
            return new UpdatePatchesResponsibilityRequest() { Id = id ?? Guid.NewGuid(), ResponsibileEntityId = responsibilityId ?? Guid.NewGuid() };
        }

        private UpdatePatchesResponsibilitiesRequestObject ConstructUpdateRequest()
        {
            var request = _fixture.Create<UpdatePatchesResponsibilitiesRequestObject>();

            return request;
        }
        private PatchesDb ConstructUpdateResponse(Guid? id)
        {
            return _fixture.Build<PatchesDb>()
                            .With(y => y.Id, id ?? Guid.NewGuid())
                            .Create();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(3)]
        public async Task UpdatePatchByResponsibilitiesUseCaseReturnsResult(int? ifMatch)
        {
            //Arrange
            var request = ConstructUpdateRequest();
            var query = ConstructQuery();
            var gatewayResponse = ConstructUpdateResponse(query.Id);

            _mockGateway.Setup(x => x.UpdatePatchResponsibilities(query, request, ifMatch)).ReturnsAsync(gatewayResponse);
            //Act
            var response = await _classUnderTest.ExecuteAsync(query, request, ifMatch).ConfigureAwait(false);
            //Assert
            response.Should().BeEquivalentTo(gatewayResponse.ToDomain().ToResponse());

        }

        [Theory]
        [InlineData(null)]
        [InlineData(3)]
        public async Task UpdatePatchByResponsibiltiesUseCaseReturnsNull(int? ifMatch)
        {
            //Arrange
            var request = ConstructUpdateRequest();
            var query = ConstructQuery();

            _mockGateway.Setup(x => x.UpdatePatchResponsibilities(query, request, ifMatch)).ReturnsAsync((PatchesDb) null);
            //Act
            var response = await _classUnderTest.ExecuteAsync(query, request, ifMatch).ConfigureAwait(false);

            //Assert
            response.Should().BeNull();

        }

        [Theory]
        [InlineData(null)]
        [InlineData(3)]
        public void UpdateTenureByIdAsyncExceptionIsThrown(int? ifMatch)
        {
            // Arrange
            var request = ConstructUpdateRequest();
            var query = ConstructQuery();
            var exception = new ApplicationException("Test exception");
            _mockGateway.Setup(x => x.UpdatePatchResponsibilities(query, request, ifMatch)).ThrowsAsync(exception);

            // Act
            Func<Task<PatchesResponseObject>> func = async () =>
                await _classUnderTest.ExecuteAsync(query, request, ifMatch).ConfigureAwait(false);

            // Assert
            func.Should().Throw<ApplicationException>().WithMessage(exception.Message);
        }
    }

}
