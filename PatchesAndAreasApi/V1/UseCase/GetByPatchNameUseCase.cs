using Hackney.Core.Logging;
using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using PatchesAndAreasApi.V1.Gateways;
using PatchesAndAreasApi.V1.UseCase.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase
{
    public class GetByPatchNameUseCase : IGetByPatchNameUseCase
    {
        private readonly IPatchesGateway _gateway;

        public GetByPatchNameUseCase(IPatchesGateway gateway)
        {
            _gateway = gateway;
        }

        [LogCall]
        public async Task<PatchEntity> ExecuteAsync(GetByPatchNameQueryV1 query)
        {

            var gatewayResult = await _gateway.GetByPatchNameAsync(query).ConfigureAwait(false);
            return gatewayResult;

        }
    }
}
