using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using PatchesApi.V1.Domain;
using PatchesApi.V1.Factories;
using PatchesApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.Tests.V1.E2ETests.Fixtures
{
    public class PatchesFixtures : IDisposable
    {
        private readonly Fixture _fixture = new Fixture();
        private readonly Random _random = new Random();
        public readonly IDynamoDBContext _dbContext;
        public PatchesDb PatchesDb { get; private set; }

        public PatchEntity ExistingPatch { get; private set; }

        public Guid Id { get; private set; }
        public string InvalidId { get; private set; }

        public PatchesFixtures(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
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
                                    .With(x => x.VersionNumber, (int?) null)
                                    .Create();

                _dbContext.SaveAsync<PatchesDb>(patch).GetAwaiter().GetResult();
                PatchesDb = patch;
                Id = patch.Id;
            }
        }

        public void GivenAPatchDoesNotExist()
        {
            Id = Guid.NewGuid();
        }

        public void GivenAnInvalidId()
        {
            InvalidId = "1234567";
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
