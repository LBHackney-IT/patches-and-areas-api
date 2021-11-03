using Amazon.DynamoDBv2.DataModel;
using AutoFixture;
using PatchesApi.V1.Boundary.Request;
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
        public string InvalidId { get; private set; }
        public Guid ResponsibleId { get; private set; }

        public UpdatePatchesResponsibilitiesRequestObject UpdateResponsibleRequestObject
        { get; private set; }

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
        }

        public void GivenAnUpdatePatchWithNewResponsibleEntityRequest()
        {
            if (null == PatchesDb)
            {
                var patch = _fixture.Build<PatchesDb>()
                                    .With(x => x.VersionNumber, (int?) null)
                                    .Create();

                _dbContext.SaveAsync<PatchesDb>(patch).GetAwaiter().GetResult();
                PatchesDb = patch;
                Id = patch.Id;

                var request = _fixture.Create<UpdatePatchesResponsibilitiesRequestObject>();
                UpdateResponsibleRequestObject = request;
                ResponsibleId = request.Id;

            }
        }

        public void GivenAnUpdatePatchWithNewResponsibleEntityRequestWithValidationError()
        {
            var request = new UpdatePatchesResponsibilitiesRequestObject();
            UpdateResponsibleRequestObject = request;
        }
    }
}
