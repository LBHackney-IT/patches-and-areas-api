using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesApi.V1.Infrastructure.Exceptions
{
    public class ResponsibilityEntityException : Exception
    {
        public Guid ResponsibilityEntityId { get; private set; }

        public ResponsibilityEntityException(Guid responsibilityId)
            : base(string.Format("The Responsibility Entity Id given doesn't exist, please provide an existing one.",
                                 responsibilityId.ToString()))
        {
            ResponsibilityEntityId = responsibilityId;
        }
    }
}
