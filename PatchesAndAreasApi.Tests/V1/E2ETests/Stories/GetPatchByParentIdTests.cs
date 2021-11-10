using PatchesAndAreasApi.Tests.V1.E2ETests.Fixtures;
using PatchesAndAreasApi.Tests.V1.E2ETests.Steps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Xunit;

namespace PatchesAndAreasApi.Tests.V1.E2ETests.Stories
{
    [Story(
          AsA = "Service",
          IWant = "an endpoint to return patch details",
          SoThat = "it is possible to view the details of a patch")]
    [Collection("DynamoDb collection")]
    public class GetPatchByParentIdTests : IDisposable
    {
        private readonly DynamoDbIntegrationTests<Startup> _dbFixture;
        private readonly PatchesFixtures _patchFixtures;
        private readonly GetPatchByParentIdStep _steps;

        public GetPatchByParentIdTests(DynamoDbIntegrationTests<Startup> dbFixture)
        {
            _dbFixture = dbFixture;
            _patchFixtures = new PatchesFixtures(_dbFixture.DynamoDbContext);
            _steps = new GetPatchByParentIdStep(_dbFixture.Client);
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
                if (null != _patchFixtures)
                    _patchFixtures.Dispose();

                _disposed = true;
            }
        }

        [Fact]
        public void ServiceReturnsTheRequestedPatch()
        {
            this.Given(g => _patchFixtures.GivenAPatchListAlreadyExists())
                .When(w => _steps.WhenPatchDetailsAreRequested(_patchFixtures.ParentId.ToString()))
                .Then(t => _steps.ThenThePatchDetailsAreReturned(_patchFixtures.PatchesDbList))
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsNotFoundIfPatchByIdDoesNotExist()
        {
            this.Given(g => _patchFixtures.GivenAPatchDoesNotExist())
                .When(w => _steps.WhenPatchDetailsAreRequested(_patchFixtures.ParentId.ToString()))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }

        [Fact]
        public void ServiceReturnsBadRequestIfIdInvalid()
        {
            this.Given(g => _patchFixtures.GivenAnInvalidId())
                .When(w => _steps.WhenPatchDetailsAreRequested(_patchFixtures.InvalidParentId))
                .Then(t => _steps.ThenBadRequestIsReturned())
                .BDDfy();
        }

    }
}
