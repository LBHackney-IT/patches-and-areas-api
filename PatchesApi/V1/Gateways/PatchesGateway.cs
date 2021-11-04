using Amazon.DynamoDBv2.DataModel;
using PatchesApi.V1.Domain;
using PatchesApi.V1.Factories;
using PatchesApi.V1.Infrastructure;
using Hackney.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using PatchesApi.V1.Boundary.Response;
using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Infrastructure.Exceptions;
using System.Linq;

namespace PatchesApi.V1.Gateways
{
    public class PatchesGateway : IPatchesGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<PatchesGateway> _logger;


        public PatchesGateway(IDynamoDBContext dynamoDbContext, ILogger<PatchesGateway> logger)
        {
            _dynamoDbContext = dynamoDbContext;
            _logger = logger;
        }

        [LogCall]
        public async Task<PatchEntity> GetPatchByIdAsync(PatchesQueryObject query)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id parameter {query.Id}");

            var result = await _dynamoDbContext.LoadAsync<PatchesDb>(query.Id).ConfigureAwait(false);
            return result?.ToDomain();
        }

        [LogCall]
        public async Task<PatchesDb> DeleteResponsibilityFromPatch(DeleteResponsibilityFromPatchRequest query)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.Id}");

            var existingPatch = await _dynamoDbContext.LoadAsync<PatchesDb>(query.Id).ConfigureAwait(false);
            if (existingPatch == null) throw new PatchNotFoundException();

            // remove person from tenure
            var initialNumberOfResponsibility = existingPatch.ResponsibleEntities.Count;
            var filteredResponsibilityEntity = existingPatch.ResponsibleEntities.Where(x => x.Id != query.ResponsibileEntityId).ToList();

            // if person was removed, the count should be less
            if (filteredResponsibilityEntity.Count == initialNumberOfResponsibility) throw new ResponsibileIdNotFoundInPatchException();

            // save changes to database
            _logger.LogDebug($"Calling IDynamoDBContext.SaveAsync to update id {query.ResponsibileEntityId}");

            existingPatch.ResponsibleEntities = filteredResponsibilityEntity;
            await _dynamoDbContext.SaveAsync(existingPatch).ConfigureAwait(false);

            return existingPatch;
        }
    }
}
