namespace robhabraken.SitecoreShrink.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Static extension methods class, used to aid in analyzing the recursively nested media item report objects.
    /// </summary>
    public static class ExtensionMethods
    {

        /// <summary>
        /// Extension method to flatten a recursively nested object.
        /// </summary>
        /// <typeparam name="T">The type of the object to flatten.</typeparam>
        /// <param name="source">The original source of the object that is being flattened.</param>
        /// <param name="recursion">The recursion result of flattening it's children.</param>
        /// <returns>A flattened enumeration of type T, containing a list of all objects found in its children.</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> recursion)
        {
            return source.SelectMany(x => recursion(x).Flatten(recursion)).Concat(source);
        }
    }
}
