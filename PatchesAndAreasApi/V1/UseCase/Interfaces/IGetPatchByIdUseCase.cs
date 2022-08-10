using PatchesAndAreas.Boundary.Request;
using PatchesAndAreas.Domain;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase.Interfaces
{
    public interface IGetPatchByIdUseCase
    {
        Task<PatchEntity> Execute(PatchesQueryObject query);
    }
}
