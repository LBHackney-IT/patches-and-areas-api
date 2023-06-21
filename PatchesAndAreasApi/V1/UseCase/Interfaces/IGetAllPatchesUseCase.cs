using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase.Interfaces
{
    public interface IGetAllPatchesUseCase
    {
        Task<List<PatchEntity>> Execute();
    }
}
