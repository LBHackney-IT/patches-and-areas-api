using PatchesAndAreas.Boundary.Request;
using PatchesAndAreas.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase.Interfaces
{
    public interface IGetPatchByParentIdUseCase
    {
        Task<List<PatchEntity>> ExecuteAsync(GetPatchByParentIdQuery query);

    }
}
