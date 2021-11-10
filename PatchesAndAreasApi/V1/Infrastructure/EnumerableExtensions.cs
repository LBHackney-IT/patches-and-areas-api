using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatchesAndAreasApi.V1.Infrastructure
{
    public static class EnumerableExtensions
    {
        public static List<T> ToListOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null ? new List<T>() : enumerable.ToList();
        }

        public static List<T> ToListOrNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null ? null : enumerable.ToList();
        }
    }
}
