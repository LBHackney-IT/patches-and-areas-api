using Amazon.DynamoDBv2.DataModel;
using Amazon.SimpleNotificationService;
using AutoFixture;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using Hackney.Shared.PatchesAndAreas.Factories;
using Hackney.Shared.PatchesAndAreas.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.Tests.V1.E2ETests.Fixtures
{
    public class PatchesFixtures : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();
        public readonly IDynamoDBContext _dbContext;
        public readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        public PatchesDb PatchesDb { get; private set; }

        public PatchEntity ExistingPatch { get; private set; }
        public List<PatchesDb> PatchesDbList { get; private set; }

        public Guid Id { get; private set; }
        public Guid ParentId { get; private set; }
        public string InvalidId { get; private set; }
        public Guid ResponsibleId { get; private set; }


        public UpdatePatchesResponsibilitiesRequestObject UpdateResponsibleRequestObject
        { get; private set; }

        public List<ResponsibleEntities> ResponsibleEntities { get; private set; }
        public ResponsibleEntities ResponsibleEntity { get; private set; }
        public string InvalidParentId { get; private set; }

        public PatchesFixtures(IDynamoDBContext dbContext, IAmazonSimpleNotificationService amazonSimpleNotificationService)
        {
            _dbContext = dbContext;
            _amazonSimpleNotificationService = amazonSimpleNotificationService;
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
                if (null != PatchesDb)
                    _dbContext.DeleteAsync<PatchesDb>(PatchesDb.Id).GetAwaiter().GetResult();

                _disposed = true;
            }
        }

        public void GivenAPatchAlreadyExists()
        {
            if (null == PatchesDb)
            {
                var patch = _fixture.Build<PatchesDb>()
                                    .Without(x => x.VersionNumber)
                                    .Create();

                _dbContext.SaveAsync<PatchesDb>(patch).GetAwaiter().GetResult();
                PatchesDb = patch;
                Id = patch.Id;
                ParentId = patch.ParentId;
            }
        }

        public void GivenAPatchListAlreadyExists()
        {
            var parentid = Guid.NewGuid();
            var patches = new List<PatchesDb>();

            patches.AddRange(_fixture.Build<PatchesDb>()
                                  .With(x => x.ParentId, parentid)
                                  .Without(x => x.VersionNumber)

                                  .CreateMany(5));

            foreach (var patch in patches)
            {
                _dbContext.SaveAsync(patch).GetAwaiter().GetResult();
            }
            PatchesDbList = patches;
            ParentId = parentid;
        }
        public void GivenAPatchDoesNotExist()
        {
            Id = Guid.NewGuid();
            ParentId = Guid.NewGuid();
        }

        public void GivenAPatchUpdateRequestDoesNotExist()
        {
            Id = Guid.NewGuid();
            var request = _fixture.Create<UpdatePatchesResponsibilitiesRequestObject>();
            UpdateResponsibleRequestObject = request;
            ResponsibleId = request.Id;
        }

        public void GivenAnInvalidId()
        {
            InvalidId = "1234567";
            InvalidParentId = "1234567890";
        }

        public void GivenAnUpdatePatchWithNewResponsibleEntityRequest()
        {
            if (null == PatchesDb)
            {
                var patch = _fixture.Build<PatchesDb>()
                                    .Without(x => x.VersionNumber)
                                    .Create();

                _dbContext.SaveAsync<PatchesDb>(patch).GetAwaiter().GetResult();
                PatchesDb = patch;
                Id = patch.Id;

                var request = _fixture.Create<UpdatePatchesResponsibilitiesRequestObject>();
                UpdateResponsibleRequestObject = request;
                ResponsibleId = request.Id;

            }
        }

        private (PatchesDb, List<ResponsibleEntities>) CreateEntityWithResponsibleEntities()
        {
            var responsibleEntityList = new List<ResponsibleEntities> { };
            responsibleEntityList.Add(_fixture.Create<ResponsibleEntities>());
            responsibleEntityList.Add(_fixture.Create<ResponsibleEntities>());

            var entity = _fixture.Build<PatchesDb>()
                                 .With(x => x.ResponsibleEntities, responsibleEntityList)
                                 .Without(x => x.VersionNumber)
                                 .Create();
            return (entity, responsibleEntityList);
        }

        public void GivenAnReplacePatchResponsibleEntitiesWithNewResponsibleEntityRequest()
        {
            if (null == PatchesDb)
            {
                var entity = CreateEntityWithResponsibleEntities();
                var patch = entity.Item1;
                var responsibleEntityList = entity.Item2;

                _dbContext.SaveAsync<PatchesDb>(patch).GetAwaiter().GetResult();
                PatchesDb = patch;
                Id = patch.Id;
                var newResponsibleEntity = _fixture.Create<ResponsibleEntities>();
                responsibleEntityList.Add(newResponsibleEntity);

                ResponsibleEntities = responsibleEntityList;
                ResponsibleEntity = newResponsibleEntity;
            }
        }

        public void GivenAReplacePatchResponsibleEntitiesWithRemovingResponsibleEntityRequest()
        {
            if (null == PatchesDb)
            {
                var entity = CreateEntityWithResponsibleEntities();
                var patch = entity.Item1;
                var responsibleEntityList = entity.Item2;

                var toBeDeletedResponsibleEntity = _fixture.Create<ResponsibleEntities>();
                responsibleEntityList.Add(toBeDeletedResponsibleEntity);

                _fixture.Build<PatchesDb>()
                        .With(x => x.ResponsibleEntities, responsibleEntityList)
                        .Without(x => x.VersionNumber)
                        .Create();

                _dbContext.SaveAsync<PatchesDb>(patch).GetAwaiter().GetResult();
                PatchesDb = patch;
                Id = patch.Id;
                responsibleEntityList.Remove(toBeDeletedResponsibleEntity);

                ResponsibleEntities = responsibleEntityList;
                ResponsibleEntity = toBeDeletedResponsibleEntity;
            }
        }

        public void GivenAnReplacePatchResponsibleEntitiesWithSameResponsibleEntityRequest()
        {
            if (null == PatchesDb)
            {
                var entity = CreateEntityWithResponsibleEntities();
                var patch = entity.Item1;
                var responsibleEntityList = entity.Item2;

                _dbContext.SaveAsync<PatchesDb>(patch).GetAwaiter().GetResult();
                PatchesDb = patch;
                Id = patch.Id;

                ResponsibleEntities = responsibleEntityList;
            }
        }

        public void GivenAnUpdatePatchWithNewResponsibleEntityRequestWithValidationError()
        {
            var request = new UpdatePatchesResponsibilitiesRequestObject();
            Id = Guid.Empty;
            UpdateResponsibleRequestObject = request;
        }

        public void GivenAPatchExistsWithNoResponsibileEntity()
        {
            var entity = _fixture.Build<PatchesDb>()
                .With(x => x.ResponsibleEntities, new List<ResponsibleEntities>())
                .With(x => x.VersionNumber, (int?) null)
                .Create();

            _dbContext.SaveAsync(entity).GetAwaiter().GetResult();

            ExistingPatch = entity.ToDomain();
            PatchesDb = entity;
            Id = entity.Id;
        }

        public void GivenAPatchExistsWithManyResponsibility()
        {
            var numberOfResponsibilities = _random.Next(2, 5);
            var responsibileEntities = _fixture.CreateMany<ResponsibleEntities>(numberOfResponsibilities).ToList();

            var entity = _fixture.Build<PatchesDb>()
                          .With(x => x.ResponsibleEntities, responsibileEntities)
                          .With(x => x.VersionNumber, (int?) null)
                          .Create();

            _dbContext.SaveAsync(entity).GetAwaiter().GetResult();

            ExistingPatch = entity.ToDomain();
            PatchesDb = entity;
            Id = entity.Id;
        }


    }
}
