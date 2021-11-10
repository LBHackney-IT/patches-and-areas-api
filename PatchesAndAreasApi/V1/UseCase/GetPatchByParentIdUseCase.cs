using Hackney.Core.Logging;
using PatchesAndAreasApi.V1.Boundary.Request;
using PatchesAndAreasApi.V1.Domain;
using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase
{
    public class GetPatchByParentIdUseCase : IGetPatchByParentIdUseCase
    {
        private readonly IPatchesGateway _gateway;

        public GetPatchByParentIdUseCase(IPatchesGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<List<PatchEntity>> ExecuteAsync(GetPatchByParentIdQuery query)
        {

            var gatewayResult = await _gateway.GetByParentIdAsync(query).ConfigureAwait(false);
            return gatewayResult;

        }
    }
}
