using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Sns;
using PatchesAndAreasApi.Tests.V1.E2ETests.Fixtures;
using PatchesAndAreasApi.Tests.V1.E2ETests.Steps;
using System;
using TestStack.BDDfy;
using Xunit;

namespace PatchesAndAreasApi.Tests.V1.E2ETests.Stories
{
    [Story(
          AsA = "Service",
          IWant = "an endpoint to return patch details",
          SoThat = "it is possible to view the details of a patch")]
    [Collection("AppTest collection")]
    public class GetPatchByParentIdTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly PatchesFixtures _patchFixtures;
        private readonly ISnsFixture _snsFixture;
        private readonly GetPatchByParentIdStep _steps;

        public GetPatchByParentIdTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _snsFixture = appFactory.SnsFixture;
            _patchFixtures = new PatchesFixtures(_dbFixture.DynamoDbContext, _snsFixture.SimpleNotificationService);
            _steps = new GetPatchByParentIdStep(appFactory.Client);
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
