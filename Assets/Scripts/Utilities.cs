using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace TableDungeon
{
    public static class Utilities
    {
        public static T[] GetEnumValues<T>() where T : Enum => (T[]) Enum.GetValues(typeof(T));

        public static bool HasComponent<T> (this GameObject obj)
        {
            return obj.GetComponent<T>() as Component != null;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T> predicate)
        {
            foreach (var value in enumerator) predicate.Invoke(value);
        }
        
        public static void ForEach<T>(this IEnumerable<T> enumerator, Action<T, int> predicate)
        {
            var i = 0;
            foreach (var value in enumerator) predicate.Invoke(value, i++);
        }

        public static float NextRange(this Random random, float min, float max)
        {
            return (float) (min + random.NextDouble() * (max - min));
        }
    }
}