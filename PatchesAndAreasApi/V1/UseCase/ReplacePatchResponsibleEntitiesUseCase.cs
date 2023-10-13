using Hackney.Core.Logging;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Boundary.Response;
using Hackney.Shared.PatchesAndAreas.Domain;
using Hackney.Shared.PatchesAndAreas.Factories;
using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase
{
    public class ReplacePatchResponsibleEntitiesUseCase : IReplacePatchResponsibleEntitiesUseCase
    {
        private readonly IPatchesGateway _gateway;
        public ReplacePatchResponsibleEntitiesUseCase(IPatchesGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<PatchesResponseObject> ExecuteAsync(PatchesQueryObject query, List<ResponsibleEntities> responsibleEntitiesRequestObject,
             int? ifMatch)
        {
            var updateResult = await _gateway.ReplacePatchResponsibleEntities(query, responsibleEntitiesRequestObject, ifMatch).ConfigureAwait(false);
            if (updateResult == null) return null;

            return updateResult.ToDomain().ToResponse();
        }
    }
}
