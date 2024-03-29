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
       AsA = "Internal Hackney user (such as a Housing Officer or Area housing Manager)",
       IWant = "the ability to add new responsible entity",
       SoThat = "I can update a patch/area")]
    [Collection("AppTest collection")]
    public class UpdateResponsibleEntityTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly PatchesFixtures _patchFixture;
        private readonly ISnsFixture _snsFixture;

        private readonly UpdatePatchResponsibilityStep _steps;

        public UpdateResponsibleEntityTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _snsFixture = appFactory.SnsFixture;
            _patchFixture = new PatchesFixtures(_dbFixture.DynamoDbContext, _snsFixture.SimpleNotificationService);
            _steps = new UpdatePatchResponsibilityStep(appFactory.Client);
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

        [Theory]
        [InlineData(null)]
        [InlineData(5)]
        public void ServiceReturnsConflictWhenIncorrectVersionNumber(int? versionNumber)
        {
            this.Given(g => _patchFixture.GivenAnUpdatePatchWithNewResponsibleEntityRequest())
                .When(w => _steps.WhenTheUpdatePatchApiIsCalled(_patchFixture.Id, _patchFixture.ResponsibleId, _patchFixture.UpdateResponsibleRequestObject, versionNumber))
                .Then(t => _steps.ThenConflictIsReturned(versionNumber))
                .BDDfy();
        }

        [Fact]
        public void ServiceUpdateTheRequestedPatchWithNewResponsibleEntity()

        {
            this.Given(g => _patchFixture.GivenAnUpdatePatchWithNewResponsibleEntityRequest())
                .And(g => _steps.WhenTheUpdatePatchApiIsCalled(_patchFixture.Id, _patchFixture.ResponsibleId, _patchFixture.UpdateResponsibleRequestObject))
                .Then(t => _steps.ThenANewResponsibilityEntityIsAdded(_patchFixture, _patchFixture.ResponsibleId, _patchFixture.UpdateResponsibleRequestObject))

                .BDDfy();
        }


        [Fact]
        public void ServiceReturnsNotFoundIfPatchNotExist()
        {
            this.Given(g => _patchFixture.GivenAPatchUpdateRequestDoesNotExist())
                .When(w => _steps.WhenTheUpdatePatchApiIsCalled(_patchFixture.Id, _patchFixture.ResponsibleId, _patchFixture.UpdateResponsibleRequestObject))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }


    }
}
