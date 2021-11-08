using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.UseCase.Interfaces
{
    public interface IUpdatePatchResponsibilitiesUseCase
    {
        Task<PatchesResponseObject> ExecuteAsync(UpdatePatchesResponsibilityRequest query, UpdatePatchesResponsibilitiesRequestObject updateRequestObject,
             int? ifMatch);
    }
}
