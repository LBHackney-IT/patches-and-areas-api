using Hackney.Core.JWT;
using Hackney.Core.Logging;
using Hackney.Core.Sns;
using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Boundary.Response;
using PatchesApi.V1.Factories;
using PatchesApi.V1.Gateways;
using PatchesApi.V1.UseCase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.UseCase
{
    public class UpdatePatchResponsibilitiesUseCase : IUpdatePatchResponsibilitiesUseCase
    {
        private readonly IPatchesGateway _gateway;
        public UpdatePatchResponsibilitiesUseCase(IPatchesGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<PatchesResponseObject> ExecuteAsync(UpdatePatchesResponsibilityRequest query, UpdatePatchesResponsibilitiesRequestObject updateRequestObject,
             int? ifMatch)
        {
            var updateResult = await _gateway.UpdatePatchResponsibilities(query, updateRequestObject, ifMatch).ConfigureAwait(false);
            if (updateResult == null) return null;


            return updateResult.ToDomain().ToResponse();
        }
    }
}
