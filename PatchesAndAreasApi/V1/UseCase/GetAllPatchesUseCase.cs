using Hackney.Core.Logging;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase
{
    public class GetAllPatchesUseCase : IGetAllPatchesUseCase
    {
        private IPatchesGateway _gateway;
        public GetAllPatchesUseCase(IPatchesGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<List<PatchEntity>> Execute()
        {
            var allPatches = await _gateway.GetAllPatchesAsync().ConfigureAwait(false);
            return allPatches;
        }
    }
}
