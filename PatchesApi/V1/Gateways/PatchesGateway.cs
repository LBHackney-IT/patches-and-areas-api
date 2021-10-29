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
using Amazon.DynamoDBv2.DocumentModel;
using System.Linq;

namespace PatchesApi.V1.Gateways
{
    public class PatchesGateway : IPatchesGateway
    {
        private readonly IDynamoDBContext _dynamoDbContext;
        private readonly ILogger<PatchesGateway> _logger;
        private const string GETPATCHBYPARENTIDINDEX = "PatchByParentId";



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
        public async Task<List<PatchEntity>> GetByParentIdAsync(GetPatchByParentIdQuery query)
        {


            var patchDb = new List<PatchesDb>();

            var filterExpression = new Expression();
            var keyExpression = new Expression();

            filterExpression.ExpressionAttributeNames.Add("#t", "parentId");
            filterExpression.ExpressionAttributeValues.Add(":parentId", query.ParentId);
            keyExpression.ExpressionStatement = "#t = :parentId";

            var table = _dynamoDbContext.GetTargetTable<PatchesDb>();
            var queryConfig = new QueryOperationConfig
            {
                IndexName = GETPATCHBYPARENTIDINDEX,
                BackwardSearch = true,
                ConsistentRead = true,
                Limit = int.MaxValue,
                FilterExpression = filterExpression,
                KeyExpression = keyExpression,
            };

            var search = table.Query(queryConfig);

            _logger.LogDebug($"Querying {queryConfig.IndexName} index for parentId {query.ParentId}");
            while (!search.IsDone)
            {
                var resultsSet = await search.GetNextSetAsync().ConfigureAwait(false);
                if (resultsSet.Any())
                {
                    patchDb.AddRange(_dynamoDbContext.FromDocuments<PatchesDb>(resultsSet));
                }
            }



            return (List<PatchEntity>) patchDb.Select(x => x.ToDomain());
        }
    }
}
