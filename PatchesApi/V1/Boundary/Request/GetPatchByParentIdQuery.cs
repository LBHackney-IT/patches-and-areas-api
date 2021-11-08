using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.Boundary.Request
{
    public class GetPatchByParentIdQuery
    {
        [FromQuery(Name = "parentId")]
        public Guid ParentId { get; set; }
    }
}
