using UnityEngine;
using System.Collections.Generic;

public static class ArrayExtensions
{
    public static T Sample<T>(this T[] arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Length)];
    }
}

public static class ColorExtensions
{
    public static Color With(this Color c, float? r = null, float? g = null, float? b = null, float? a = null)
    {
        return new Color(r ?? c.r, g ?? c.g, b ?? c.b, a ?? c.a);
    }
}