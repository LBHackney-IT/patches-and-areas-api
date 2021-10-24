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
        public async Task<Entity> GetEntityById(PatchesQueryObject query)
        {
            _logger.LogDebug($"Calling IDynamoDBContext.LoadAsync for id parameter {query.Id}");

            var result = await _dynamoDbContext.LoadAsync<DatabaseEntity>(query.Id).ConfigureAwait(false);
            return result?.ToDomain();
        }
    }
}
