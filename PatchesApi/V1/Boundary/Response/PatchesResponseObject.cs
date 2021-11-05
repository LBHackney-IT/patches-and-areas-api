using PatchesApi.V1.Domain;
using System;

namespace PatchesApi.V1.Boundary.Response
{
    public class PatchesResponseObject
    {
        public Guid Id { get; set; }
        public Guid ParentId { get; set; }
        public string Name { get; set; }
        public PatchType PatchType { get; set; }
        public string Domain { get; set; }
        public ResponsibleEntities ResponsibleEntities { get; set; }

    }
}
