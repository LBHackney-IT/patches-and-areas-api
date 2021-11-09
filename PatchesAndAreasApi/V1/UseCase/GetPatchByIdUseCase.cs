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
    public class GetPatchByIdUseCase : IGetPatchByIdUseCase
    {
        private IPatchesGateway _gateway;
        public GetPatchByIdUseCase(IPatchesGateway gateway)
        {
            _gateway = gateway;
        }
        [LogCall]
        public async Task<PatchEntity> Execute(PatchesQueryObject query)
        {
            var patch = await _gateway.GetPatchByIdAsync(query).ConfigureAwait(false);
            return patch;
        }
    }
}
