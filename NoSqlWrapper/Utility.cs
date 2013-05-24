using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoSqlWrapper
{
    internal static class Utility
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
        public static void TryDispose(this IDisposable disposable)
        {
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
