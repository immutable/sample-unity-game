using System;
using UnityEngine;

namespace HyperCasual.Runner
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this T[] array)
        {
            // Need a wrapper to serialise arrays
            var wrapper = new Wrapper<T>();
            wrapper.Items = array;
            var wrapped = JsonUtility.ToJson(wrapper);
            // Remove the wrapper
            return wrapped.ReplaceFirst("{\"Items\":", "").ReplaceLast("}", "");
        }

        private static string ReplaceFirst(this string source, string search, string replace)
        {
            var pos = source.IndexOf(search);
            if (pos < 0) return source;
            return source.Substring(0, pos) + replace + source.Substring(pos + search.Length);
        }

        private static string ReplaceLast(this string source, string search, string replace)
        {
            var place = source.LastIndexOf(search);
            if (place == -1) return source;
            return source.Remove(place, search.Length).Insert(place, replace);
        }

        [Serializable]
        public class Wrapper<T>
        {
            public T[] Items;
        }
    }
}