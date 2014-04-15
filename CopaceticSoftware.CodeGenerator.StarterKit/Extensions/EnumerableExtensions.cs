using System;
using System.Collections.Generic;

namespace CopaceticSoftware.CodeGenerator.StarterKit.Extensions
{
    public static class EnumerableExtensions
    {
        public static void Map<T>(this IEnumerable<T> collection, Action<T> func)
        {
            if (null == collection)
                return;

            if (null == func)
                return;

            foreach (var item in collection)
                func(item);
        }
    }
}
