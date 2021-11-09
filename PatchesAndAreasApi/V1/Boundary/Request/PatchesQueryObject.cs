using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.Boundary.Request
{
    public class PatchesQueryObject
    {
        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
    }
}
