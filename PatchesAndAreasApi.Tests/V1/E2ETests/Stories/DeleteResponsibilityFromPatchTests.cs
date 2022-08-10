using AutoFixture;
using Hackney.Core.Testing.DynamoDb;
using PatchesAndAreas.Boundary.Request;
using PatchesAndAreasApi.Tests.V1.E2ETests.Fixtures;
using PatchesAndAreasApi.Tests.V1.E2ETests.Steps;
using System;
using System.Linq;
using TestStack.BDDfy;
using Xunit;

namespace PatchesAndAreasApi.Tests.V1.E2ETests.Stories
{
    [Story(
       AsA = "Internal Hackney user (such as a Housing Officer or Area housing Manager)",
       IWant = "the ability to delete a responsibility from a patch/area",
       SoThat = " can end their relationship to the patch/area"
   )]
    [Collection("AppTest collection")]
    public class DeleteResponsibilityFromPatchTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;

        private readonly PatchesFixtures _patchFixture;
        private readonly DeleteResponsibilityFromPatchStep _steps;
        private readonly Fixture _fixture = new Fixture();

        public DeleteResponsibilityFromPatchTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _patchFixture = new PatchesFixtures(_dbFixture.DynamoDbContext);
            _steps = new DeleteResponsibilityFromPatchStep(appFactory.Client);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                if (null != _patchFixture)
                    _patchFixture.Dispose();
                if (null != _steps)
                    _steps.Dispose();
                _disposed = true;
            }
        }

        [Fact]
        public void ServiceReturns404WhenPatchDoesntExist()
        {
            var query = _fixture.Create<DeleteResponsibilityFromPatchRequest>();

            this.Given(g => _patchFixture.GivenAPatchDoesNotExist())
                .When(w => _steps.WhenDeleteResponsibilityFromPatchApiIsCalledAsync(query))
                .Then(t => _steps.NotFoundResponseReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturns404WhenResponsibilityIdDoesntExistInPatch()
        {
            this.Given(g => _patchFixture.GivenAPatchExistsWithNoResponsibileEntity())
                .When(w => _steps.WhenDeleteResponsibilityFromPatchApiIsCalledAsync(new DeleteResponsibilityFromPatchRequest
                {
                    Id = _patchFixture.Id,
                    ResponsibileEntityId = _fixture.Create<Guid>()
                }))
                .Then(t => _steps.NotFoundResponseReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturns204WhenResponsibilityWasRemovedFromPatch()
        {
            // patch and responsibility exist
            this.Given(g => _patchFixture.GivenAPatchExistsWithManyResponsibility())
                .When(w => _steps.WhenDeleteResponsibilityFromPatchApiIsCalledAsync(new DeleteResponsibilityFromPatchRequest
                {
                    Id = _patchFixture.Id,
                    ResponsibileEntityId = _patchFixture.PatchesDb.ResponsibleEntities.First().Id
                }))
                .Then(t => _steps.NoContentResponseReturned())
                .And(a => _steps.ResponsibilityRemovedFromPatch(_patchFixture.Id, _patchFixture.PatchesDb.ResponsibleEntities.First().Id, _patchFixture))
                .BDDfy();
        }

    }
}
