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
using System;

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
        public async Task<PatchesDb> UpdatePatchResponsibilities(UpdatePatchesResponsibilityRequest query, UpdatePatchesResponsibilitiesRequestObject requestObject,
                                                                                         int? ifMatch)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id {query.Id} and then IDynamoDBContext.SaveAsync");
            var patch = await _dynamoDbContext.LoadAsync<PatchesDb>(query.Id).ConfigureAwait(false);
            if (patch == null) return null;
            if (ifMatch != patch.VersionNumber)
                throw new VersionNumberConflictException(ifMatch, patch.VersionNumber);
            var responsibleEntity = new ResponsibleEntities()
            {
                Id = query.ResponsibileEntityId,
                Name = requestObject.Name,
                ResponsibleType = requestObject.ResponsibleType
            };
            patch.ResponsibleEntities.Add(responsibleEntity);




            await _dynamoDbContext.SaveAsync(patch).ConfigureAwait(false);



            return patch;
        }

    }
}
