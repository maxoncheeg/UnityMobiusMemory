using System;
using System.Collections.Generic;

namespace Model.Cards
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this List<T> collection, Random random, int steps = 10)
        {
            //ArgumentNullException.ThrowIfNull(collection);
            int count = collection.Count;

            for (int i = 0; i < steps; i++)
                for (int j = 0, r = random.Next(count); j < count; j++, r = random.Next(count))
                    (collection[r], collection[j]) = (collection[j], collection[r]);
        }

        public static void Shuffle<T>(this List<List<T>> collection, Random random, int steps = 10)
        {
            int count = collection.Count;
            for (int i = 0; i < steps; i++)
                for (int j = 0; j < collection.Count; j++)
                    for (int k = 0; k < collection[j].Count; k++)
                    {
                        int u = random.Next(collection.Count),
                            w = random.Next(collection[u].Count);
                        (collection[u][w], collection[j][k]) = (collection[j][k], collection[u][w]);
                    }
        }
    }
}