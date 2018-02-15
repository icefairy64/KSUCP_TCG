using System;
using System.Collections.Generic;
using System.Linq;
namespace Server
{
    public static class Extensions
    {
        public static IEnumerable<T> Unwrap<T>(this IEnumerable<GameObject<T>> enumerable)
        {
            return enumerable.Select(x => x.Instance);
        }

        public static bool Remove<T>(this IList<GameObject<T>> list, T instance) where T : class
        {
            var obj = list.Single(x => x.Instance == instance);
            return list.Remove(obj);
        }

        public static void Replace<T>(this IList<T> list, T instance, T newInstance) where T : class
        {
            var index = list.IndexOf(instance);
            list.RemoveAt(index);
            list.Insert(index, newInstance);
        }

        /// <summary>
        /// Copies maximum available amount of items from this array to another.
        /// </summary>
        /// <returns>Amount of items copied.</returns>
        /// <param name="src">Source.</param>
        /// <param name="dest">Destination.</param>
        /// <param name="srcOffset">Source offset.</param>
        /// <param name="destOffset">Destination offset.</param>
        /// <param name="count">Amount of items to copy.</param>
        /// <typeparam name="T">Array item type.</typeparam>
        public static int CopyTo<T>(this T[] src, T[] dest, int srcOffset, int destOffset, int count)
        {
            var srcDeficit = Math.Min(srcOffset + count, src.Length) - src.Length;
            var destDeficit = Math.Min(destOffset + count, dest.Length) - dest.Length;
            var available = count - Math.Max(srcDeficit, destDeficit);
            for (int i = 0; i < available; i++)
                dest[destOffset + i] = src[srcOffset + i];
            return available;
        }
    }
}
