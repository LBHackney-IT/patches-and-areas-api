using Hackney.Core.Logging;
using PatchesAndAreasApi.V1.Boundary.Request;
using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase
{
    public class DeleteResponsibilityFromPatchUseCase : IDeleteResponsibilityFromPatchUseCase
    {
        private readonly IPatchesGateway _patchGateway;


        public DeleteResponsibilityFromPatchUseCase(IPatchesGateway patchesGateway)
        {
            _patchGateway = patchesGateway;

        }

        [LogCall]
        public async Task Execute(DeleteResponsibilityFromPatchRequest query)
        {
            var result = await _patchGateway.DeleteResponsibilityFromPatch(query).ConfigureAwait(false);
        }
    }
}
