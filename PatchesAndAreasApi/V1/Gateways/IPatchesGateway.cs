using PatchesAndAreasApi.V1.Boundary.Request;
using PatchesAndAreasApi.V1.Boundary.Response;
using PatchesAndAreasApi.V1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.Gateways
{
    public interface IPatchesGateway
    {
        Task<PatchEntity> GetPatchByIdAsync(PatchesQueryObject query);

    }
}
