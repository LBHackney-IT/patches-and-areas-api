using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.UseCase.Interfaces
{
    public interface IGetPatchByParentIdUseCase
    {
        Task<List<PatchEntity>> ExecuteAsync(GetPatchByParentIdQuery query);

    }
}
