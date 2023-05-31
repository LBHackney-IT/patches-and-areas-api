using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using Hackney.Shared.PatchesAndAreas.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.Gateways
{
    public interface IPatchesGateway
    {
        Task<List<PatchEntity>> GetAllPatchesAsync();
        Task<PatchEntity> GetPatchByIdAsync(PatchesQueryObject query);
        Task<PatchesDb> UpdatePatchResponsibilities(UpdatePatchesResponsibilityRequest query, UpdatePatchesResponsibilitiesRequestObject requestObject, int? ifMatch);
        Task<List<PatchEntity>> GetByParentIdAsync(GetPatchByParentIdQuery query);
        Task<PatchesDb> DeleteResponsibilityFromPatch(DeleteResponsibilityFromPatchRequest query);

    }
}
