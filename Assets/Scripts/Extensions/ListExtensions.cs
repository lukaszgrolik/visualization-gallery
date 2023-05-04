using UnityEngine;
using System.Collections.Generic;

public static class IReadOnlyListExtensions
{
    // IReadOnlyList doesn't have Contains method
    public static bool Contains<T>(this IReadOnlyList<T> list, T value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var val = list[i];

            if (val.Equals(value)) return true;
        }

        return false;
    }
}

public static class ListExtensions
{
    public static T Last<T>(this IReadOnlyList<T> list)
    {
        if (list.Count == 0) return default(T);

        return list[list.Count - 1];
    }

    public static void RemoveMany<T>(this List<T> list, IReadOnlyList<T> otherList)
    {
        for (int i = 0; i < otherList.Count; i++)
        {
            list.Remove(otherList[i]);
        }
    }

    public static T Sample<T>(this IReadOnlyList<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    // http://answers.unity.com/answers/1566405/view.html
    public static IReadOnlyList<T> RandomMany<T>(this IReadOnlyList<T> list, int number, System.Random prng)
    {
        // this is the list we're going to remove picked items from
        List<T> tmpList = new List<T>(list);
        // this is the list we're going to move items to
        List<T> newList = new List<T>();

        // make sure tmpList isn't already empty
        while (newList.Count < number && tmpList.Count > 0)
        {
            int index;
            if (prng != null)
                index = prng.Next(0, tmpList.Count);
            else
                index = UnityEngine.Random.Range(0, tmpList.Count);

            newList.Add(tmpList[index]);
            tmpList.RemoveAt(index);
        }

        return newList;
    }

    public static IReadOnlyList<T> Intersection<T>(this IReadOnlyList<T> list, IReadOnlyList<T> otherList)
    {
        var result = new List<T>();

        for (int i = 0; i < list.Count; i++)
        {
            if (otherList.Contains(list[i]))
            {
                result.Add(list[i]);
            }
        }

        return result;
    }
}