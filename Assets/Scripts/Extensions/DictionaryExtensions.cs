using UnityEngine;
using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static Dictionary<K, V> Clone<K, V>(this IReadOnlyDictionary<K, V> dict)
    {
        var newDict = new Dictionary<K, V>();

        foreach (var item in dict)
        {
            newDict.Add(item.Key, item.Value);
        }

        return newDict;
    }

    public static void AddMany<K, V>(this Dictionary<K, V> dict, IReadOnlyDictionary<K, V> values)
    {
        foreach (var item in values)
        {
            dict.Add(item.Key, item.Value);
        }
    }

    public static void AddMany<K, V>(this Dictionary<K, V> dict, IReadOnlyList<K> keys, V value)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            dict.Add(keys[i], value);
        }
    }

    public static void RemoveMany<K, V>(this Dictionary<K, V> dict, IReadOnlyList<K> keys)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            dict.Remove(keys[i]);
        }
    }
}