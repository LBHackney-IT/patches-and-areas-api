using System;

namespace PatchesApi.V1.Domain
{
    //TODO: rename this class to be the domain object which this API will getting. e.g. Residents or Claimants
    public class Entity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }

        //TODO: Add fields which you are interested in
    }
}
