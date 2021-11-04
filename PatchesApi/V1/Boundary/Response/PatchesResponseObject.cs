using PatchesApi.V1.Domain;
using System;
using System.Collections.Generic;

namespace PatchesApi.V1.Boundary.Response
{
    public class PatchesResponseObject
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public PatchType PatchType { get; set; }
        public string Domain { get; set; }
        public List<ResponsibleEntities> ResponsibleEntities { get; set; }

    }
}
