using System.Collections.Generic;
using System.Linq;
using PatchesApi.V1.Boundary.Response;
using PatchesApi.V1.Domain;

namespace PatchesApi.V1.Factories
{
    public static class ResponseFactory
    {
        //TODO: Map the fields in the domain object(s) to fields in the response object(s).
        // More information on this can be found here https://github.com/LBHackney-IT/lbh-patches-api/wiki/Factory-object-mappings
        public static PatchesResponseObject ToResponse(this Entity domain)
        {
            return new PatchesResponseObject();
        }

        public static List<PatchesResponseObject> ToResponse(this IEnumerable<Entity> domainList)
        {
            return domainList.Select(domain => domain.ToResponse()).ToList();
        }
    }
}
