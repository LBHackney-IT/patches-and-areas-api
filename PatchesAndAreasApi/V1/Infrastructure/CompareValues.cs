using System.Collections.Generic;
using System;
using Hackney.Shared.PatchesAndAreas.Domain;

namespace PatchesAndAreasApi.V1.Infrastructure
{
    public static class CompareValues
    {
        /// <summary>
        ///   Checks whether all items in the enumerable are same 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="responsibleEntitiesRequestObject">The request object sent from the client.</param>
        /// <returns>
        ///   Returns true if there is 0 or 1 item in the enumerable or if all items in the enumerable are same (equal to
        ///   each other) otherwise false.
        /// </returns>
        public static bool AreAllSame<T>(this IEnumerable<T> enumerable, IEnumerable<ResponsibleEntities> responsibleEntitiesRequestObject)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

            using (var enumerator = enumerable.GetEnumerator())
            {
                var toCompare = default(T);
                if (enumerator.MoveNext())
                {
                    toCompare = enumerator.Current;
                }

                while (enumerator.MoveNext())
                {
                    if (toCompare != null && !toCompare.Equals(enumerator.Current))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
