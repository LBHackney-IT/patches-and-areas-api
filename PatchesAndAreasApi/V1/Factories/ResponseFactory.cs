using System.Collections.Generic;
using System.Linq;
using PatchesAndAreasApi.V1.Boundary.Response;
using PatchesAndAreasApi.V1.Domain;
using PatchesAndAreasApi.V1.Infrastructure;

namespace PatchesAndAreasApi.V1.Factories
{
    public static class ResponseFactory
    {
        public static PatchesResponseObject ToResponse(this PatchEntity domain)
        {
            return new PatchesResponseObject
            {
                Id = domain.Id,
                ParentId = domain.ParentId,
                Name = domain.Name,
                Domain = domain.Domain,
                PatchType = domain.PatchType,
                ResponsibleEntities = domain.ResponsibleEntities.ToListOrEmpty()
            };
        }

        public static List<PatchesResponseObject> ToResponse(this IEnumerable<PatchEntity> domainList)
        {
            if (null == domainList) return new List<PatchesResponseObject>();

            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
