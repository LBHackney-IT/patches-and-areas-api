using Hackney.Core.Logging;
using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Domain;
using PatchesApi.V1.Gateways;
using PatchesApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.UseCase
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
