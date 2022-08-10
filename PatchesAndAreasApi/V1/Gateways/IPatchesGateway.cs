using PatchesAndAreas.Boundary.Request;
using PatchesAndAreas.Domain;
using PatchesAndAreas.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.Gateways
{
    public interface IPatchesGateway
    {
        Task<PatchEntity> GetPatchByIdAsync(PatchesQueryObject query);
        Task<PatchesDb> UpdatePatchResponsibilities(UpdatePatchesResponsibilityRequest query, UpdatePatchesResponsibilitiesRequestObject requestObject,
                                                                                         int? ifMatch);
        Task<List<PatchEntity>> GetByParentIdAsync(GetPatchByParentIdQuery query);


        Task<PatchesDb> DeleteResponsibilityFromPatch(DeleteResponsibilityFromPatchRequest query);

    }
}
