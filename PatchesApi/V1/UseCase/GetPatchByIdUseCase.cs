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
