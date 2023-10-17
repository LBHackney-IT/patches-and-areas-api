using System;

namespace PatchesAndAreasApi.V1.Infrastructure
{
    public class NoChangesException : Exception
    {
        public NoChangesException()
            : base(string.Format("The responsible entity is the same as what is currently in the database"))
        {

        }
    }
}
