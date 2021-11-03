using PatchesApi.V1.Boundary.Request;
using PatchesApi.V1.Boundary.Response;
using PatchesApi.V1.Domain;
using System.Threading.Tasks;

namespace PatchesApi.V1.UseCase.Interfaces
{
    public interface IGetPatchByIdUseCase
    {
        Task<PatchEntity> Execute(PatchesQueryObject query);
    }
}