using Hackney.Core.Testing.DynamoDb;
using Hackney.Core.Testing.Sns;
using Hackney.Shared.PatchesAndAreas.Domain;
using PatchesAndAreasApi.Tests.V1.E2ETests.Fixtures;
using PatchesAndAreasApi.Tests.V1.E2ETests.Steps;
using System;
using System.Collections.Generic;
using TestStack.BDDfy;
using Xunit;

namespace PatchesAndAreasApi.Tests.V1.E2ETests.Stories
{
    [Story(
       AsA = "Internal Hackney user (such as a Housing Officer or Area housing Manager)",
       IWant = "the ability to add new responsible entity",
       SoThat = "I can update a patch/area")]
    [Collection("AppTest collection")]
    public class ReplacePatchResponsibleEntitiesTests : IDisposable
    {
        private readonly IDynamoDbFixture _dbFixture;
        private readonly ISnsFixture _snsFixture;
        private readonly PatchesFixtures _patchFixture;
        private readonly ReplacePatchResponsibleEntitiesStep _steps;

        public ReplacePatchResponsibleEntitiesTests(MockWebApplicationFactory<Startup> appFactory)
        {
            _dbFixture = appFactory.DynamoDbFixture;
            _snsFixture = appFactory.SnsFixture;
            _patchFixture = new PatchesFixtures(_dbFixture.DynamoDbContext, _snsFixture.SimpleNotificationService);
            _steps = new ReplacePatchResponsibleEntitiesStep(appFactory.Client);
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
                _snsFixture?.PurgeAllQueueMessages();
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

            this.Given(g => _patchFixture.GivenAnReplacePatchResponsibleEntitiesWithNewResponsibleEntityRequest())
                .When(w => _steps.WhenTheReplaceResponsibilityEntityApiIsCalled(_patchFixture.Id, _patchFixture.ResponsibleEntities, versionNumber))
                .Then(t => _steps.ThenConflictIsReturned(versionNumber))
                .BDDfy();
        }

        [Fact]
        public void ServiceUpdateTheRequestedPatchWithNewResponsibleEntity()
        {
            this.Given(g => _patchFixture.GivenAnReplacePatchResponsibleEntitiesWithNewResponsibleEntityRequest())
                .And(g => _steps.WhenTheReplaceResponsibilityEntityApiIsCalled(_patchFixture.Id, _patchFixture.ResponsibleEntities))
                .Then(t => _steps.ThenTheResponsibilityEntityIsReplacedWithEntitySentFromClient(_patchFixture, _patchFixture.ResponsibleEntities, _patchFixture.ResponsibleEntity))
                .Then(t => _steps.ThenThePatchOrAreaResEntityEditedEventIsRaised(_patchFixture, _snsFixture))
                .BDDfy();
        }


        [Fact]
        public void ServiceUpdateTheRequestedPatchWhenResponsibleEntityIsRemoved()
        {
            this.Given(g => _patchFixture.GivenAReplacePatchResponsibleEntitiesWithRemovingResponsibleEntityRequest())
                .And(g => _steps.WhenTheReplaceResponsibilityEntityApiIsCalled(_patchFixture.Id, _patchFixture.ResponsibleEntities))
                .Then(t => _steps.ThenTheResponsibilityEntityIsReplacedWithEntitySentFromClient(_patchFixture, _patchFixture.ResponsibleEntities, _patchFixture.ResponsibleEntity))
                .Then(t => _steps.ThenThePatchOrAreaResEntityEditedEventIsRaised(_patchFixture, _snsFixture))
                .BDDfy();
        }


        [Fact]
        public void ServiceReturnsNotFoundIfPatchNotExist()
        {
            var responsibleEntityList = new List<ResponsibleEntities> { };

            this.Given(g => _patchFixture.GivenAPatchDoesNotExist())
                .When(w => _steps.WhenTheReplaceResponsibilityEntityApiIsCalled(_patchFixture.Id, responsibleEntityList))
                .Then(t => _steps.ThenNotFoundIsReturned())
                .BDDfy();
        }


    }
}
