using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Domain;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase.Interfaces
{
    public interface IGetByPatchNameUseCase
    {
        Task<PatchEntity> ExecuteAsync(GetByPatchNameQuery query);
    }
}
