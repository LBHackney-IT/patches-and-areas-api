using PatchesAndAreasApi.V1.Boundary.Request;
using PatchesAndAreasApi.V1.Boundary.Response;
using PatchesAndAreasApi.V1.Domain;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase.Interfaces
{
    public interface IGetPatchByIdUseCase
    {
        Task<PatchEntity> Execute(PatchesQueryObject query);
    }
}
