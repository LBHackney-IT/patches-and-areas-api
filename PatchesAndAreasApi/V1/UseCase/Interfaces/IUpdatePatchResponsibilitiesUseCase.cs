using PatchesAndAreas.Boundary.Request;
using PatchesAndAreas.Boundary.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase.Interfaces
{
    public interface IUpdatePatchResponsibilitiesUseCase
    {
        Task<PatchesResponseObject> ExecuteAsync(UpdatePatchesResponsibilityRequest query, UpdatePatchesResponsibilitiesRequestObject updateRequestObject,
             int? ifMatch);
    }
}
