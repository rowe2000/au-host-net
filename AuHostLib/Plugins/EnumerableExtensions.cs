using System.Collections.Generic;
using System.Linq;

namespace AuHost.Plugins
{
    public static class EnumerableExtensions
    {
        public static T[] ToMany<T>(this T item) => new T[] {item};

        public static IEnumerable<T> ToRepeatable<T>(this IEnumerable<T> items) => items as IList<T> ?? items.ToArray();
    }
}