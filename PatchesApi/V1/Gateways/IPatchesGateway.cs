using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Boundary.Response;
using PatchesApi.V1.Domain;
using PatchesApi.V1.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.Gateways
{
    public interface IPatchesGateway
    {
        Task<PatchEntity> GetPatchByIdAsync(PatchesQueryObject query);
        Task<PatchesDb> UpdatePatchResponsibilities(UpdatePatchesResponsibilityRequest query, UpdatePatchesResponsibilitiesRequestObject requestObject,
                                                                                         int? ifMatch);

    }
}
