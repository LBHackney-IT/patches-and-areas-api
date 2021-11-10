using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.Boundary.Request
{
    public class DeleteResponsibilityFromPatchRequest
    {

        [FromRoute(Name = "id")]
        public Guid Id { get; set; }
        [FromRoute(Name = "responsibileEntityId")]
        public Guid ResponsibileEntityId { get; set; }
    }
}
