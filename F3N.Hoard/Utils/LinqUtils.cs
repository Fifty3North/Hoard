using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace F3N.Hoard.Utils
{
    public static class LinqUtils
    {

        public static IEnumerable<TResult> SelectWithIndex<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, int, TResult> selector)
        {
            int i = 0;

            foreach (var item in source)
            {
                yield return selector(item, i++);
            }
        }

        public static void ForEach<K, V>(this Dictionary<K, V> dict, Action<K, V, int> action)
        {
            int i = 0;

            foreach (var kvp in dict)
            {
                action(kvp.Key, kvp.Value, i++);
            }
        }

        public static void ForEach<K, V>(this Dictionary<K, V> dict, Action<K, V> action)
        {
            dict.ForEach((k, v, i) => action(k, v));
        }

        public static void ForEach<K>(this IEnumerable<K> enumeration, Action<K, int> action)
        {
            int i = 0;

            foreach (var item in enumeration)
            {
                action(item, i++);
            }
        }

        public static Task ForEach<K>(this IEnumerable<K> enumeration, Func<K, int, Task> action)
        {
            int i = 0;
            var tasks = enumeration.Select(async item => await action(item, i++));
            return Task.WhenAll(tasks);
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            enumeration.ForEach((s, i) => action(s));
        }

        public static async Task ForEach<T>(this IEnumerable<T> enumeration, Func<T, Task> action)
        {
            await enumeration.ForEach(async (s, i) => await action(s));
        }


        public static TResult Find<TKey, TResult>(this IDictionary<TKey, TResult> dictionary, TKey key) where TResult : class
        {
            TResult result;
            return !dictionary.TryGetValue(key, out result) ? null : result;
        }


        public static void Insert<T>(this ObservableCollection<T> coll, T val, Func<T, T, bool> less)
        {
            for (int i = 0; i < coll.Count; ++i)
            {
                if (less(val, coll[i]))
                {
                    coll.Insert(i, val);
                    return;
                }
            }

            coll.Add(val);
        }

        public static void MoveItemInList<T>(this List<T> collection, Predicate<T> itemPredicate, int newIndex)
        {
            int currentIdx = collection.FindIndex(itemPredicate);

            if (currentIdx != -1)
            {
                if (currentIdx != newIndex)
                {
                    if (newIndex >= 0 && newIndex < collection.Count)
                    {
                        T item = collection[currentIdx];

                        collection.RemoveAt(currentIdx);

                        collection.Insert(newIndex, item);
                    }
                }
            }
        }
    }
}
