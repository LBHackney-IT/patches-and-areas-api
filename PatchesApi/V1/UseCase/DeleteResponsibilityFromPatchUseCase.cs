using Hackney.Core.Logging;
using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Gateways;
using PatchesApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.UseCase
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
