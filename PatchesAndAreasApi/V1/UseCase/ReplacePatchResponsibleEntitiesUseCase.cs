using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Hackney.Core.Sns;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Boundary.Response;
using Hackney.Shared.PatchesAndAreas.Domain;
using Hackney.Shared.PatchesAndAreas.Factories;
using PatchesAndAreasApi.V1.Factories;
using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase
{
    public class ReplacePatchResponsibleEntitiesUseCase : IReplacePatchResponsibleEntitiesUseCase
    {
        private readonly IPatchesGateway _gateway;
        private readonly ISnsGateway _snsGateway;
        private readonly ISnsFactory _snsFactory;
        public ReplacePatchResponsibleEntitiesUseCase(IPatchesGateway gateway, ISnsGateway snsGateway, ISnsFactory snsFactory)
        {
            _gateway = gateway;
            _snsFactory = snsFactory;
            _snsGateway = snsGateway;

        }

        [LogCall]
        public async Task<PatchesResponseObject> ExecuteAsync(PatchesQueryObject query, List<ResponsibleEntities> responsibleEntitiesRequestObject,
             int? ifMatch, Token token)
        {
            var updateResult = await _gateway.ReplacePatchResponsibleEntities(query, responsibleEntitiesRequestObject, ifMatch).ConfigureAwait(false);
            if (updateResult == null) return null;

            var patchSnsMessage = _snsFactory.Update(updateResult, token);
            var tokenArn = Environment.GetEnvironmentVariable("PATCHES_AND_AREAS_SNS_ARN");
            await _snsGateway.Publish(patchSnsMessage, tokenArn).ConfigureAwait(false);

            return updateResult.ToDomain().ToResponse();
        }
    }
}
