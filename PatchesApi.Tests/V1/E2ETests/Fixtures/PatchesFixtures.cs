using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
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
        public readonly IDynamoDBContext _dbContext;
        public PatchesDb PatchesDb { get; private set; }

        public Guid Id { get; private set; }
        public Guid ParentId { get; private set; }
        public string InvalidId { get; private set; }
        public string InvalidParentId { get; private set; }

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
                ParentId = patch.ParentId;
            }
        }

        public void GivenAPatchDoesNotExist()
        {
            Id = Guid.NewGuid();
            ParentId = Guid.NewGuid();
        }

        public void GivenAnInvalidId()
        {
            InvalidId = "1234567";
            InvalidParentId = "1234567890";
        }


    }
}
