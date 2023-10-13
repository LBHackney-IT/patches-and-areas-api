using Hackney.Shared.PatchesAndAreas.Boundary.Request;
using Hackney.Shared.PatchesAndAreas.Boundary.Response;
using Hackney.Shared.PatchesAndAreas.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.UseCase.Interfaces
{
    public interface IReplacePatchResponsibleEntitiesUseCase
    {
        Task<PatchesResponseObject> ExecuteAsync(PatchesQueryObject query, List<ResponsibleEntities> responsibleEntitiesRequestObject,
             int? ifMatch);
    }
}
