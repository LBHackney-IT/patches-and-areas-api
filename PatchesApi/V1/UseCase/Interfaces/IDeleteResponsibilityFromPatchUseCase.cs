using PatchesApi.V1.Boundary.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.UseCase.Interfaces
{
    public interface IDeleteResponsibilityFromPatchUseCase
    {
        Task Execute(DeleteResponsibilityFromPatchRequest query);
    }
}
