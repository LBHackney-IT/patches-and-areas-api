using PatchesAndAreasApi.V1.Boundary.Request;
using PatchesAndAreasApi.V1.Boundary.Response;
using PatchesAndAreasApi.V1.Domain;
using PatchesAndAreasApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
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
