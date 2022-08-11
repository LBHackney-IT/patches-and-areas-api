using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
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
