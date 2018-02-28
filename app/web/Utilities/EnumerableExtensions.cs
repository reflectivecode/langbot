using System;
using System.Collections.Generic;
using System.Linq;

namespace LangBot.Web.Utilities
{
    public static class EnumerableExtensions
    {
        public static (T element, int index) FirstOrDefaultWithIndex<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var index = -1;
            foreach (var item in collection)
            {
                index++;
                if (predicate(item)) return (item, index);
            }
            return (default(T), -1);
        }

        public static IEnumerable<string> TrimAll(this IEnumerable<string> collection)
        {
            return collection.Select(x => x.Trim());
        }

        public static List<T> PadToLength<T>(this IEnumerable<T> collection, int length, T value = default(T))
        {
            var list = new List<T>(length);
            list.AddRange(collection);
            while (list.Count < length)
                list.Add(value);
            return list;
        }

        public static IEnumerable<TOut> SelectWithIndex<TIn, TOut>(this IEnumerable<TIn> collection, Func<TIn, int, TOut> selector)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            int index = -1;
            foreach (var item in collection)
            {
                index++;
                yield return selector(item, index);
            }
        }

        public static T GetItemAfter<T>(this IList<T> collection, T item)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            var index = collection.IndexOf(item);
            var nextIndex = (index + 1) % collection.Count;
            return collection[nextIndex];
        }
    }
}
