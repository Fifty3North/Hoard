using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace F3N.Hoard
{
    public static class CollectionExtensions
    {
        public static void Insert<T>(this IList<T> coll, T val, Func<T, T, bool> less)
        {
            int i = 0;
            foreach (var item in coll)
            {
                if (less(val, item))
                {
                    coll.Insert(i, val);
                    return;
                }
                i++;
            }

            coll.Add(val);
        }

        public static int FirstIndexOf<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            int index = -1;

            if (collection != null)
            {
                int i = 0;
                foreach(var item in collection)
                {
                    if (item != null && predicate(item))
                    {
                        index = i;
                        break;
                    }
                    i++;
                }
            }

            return index;
        }
    }
}
