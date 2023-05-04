using UnityEngine;
using System.Collections.Generic;

public static class Vector3Extensions
{
    public static Vector3 With(this Vector3 origin, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? origin.x, y ?? origin.y, z ?? origin.z);
    }

    public static Vector3 Snap(this Vector3 input, float factor = 1f)
    {
        if (factor <= 0f) throw new UnityException("factor argument must be above 0");

        float x = Mathf.Round(input.x / factor) * factor;
        float y = Mathf.Round(input.y / factor) * factor;
        float z = Mathf.Round(input.z / factor) * factor;

        return new Vector3(x, y, z);
    }
}

public static class Vector2Extensions
{
    public static Vector3 ToVector3(this Vector2 origin)
    {
        return new Vector3(origin.x, 0, origin.y);
    }
}

public static class Vector2IntExtensions
{
    public static Vector3Int ToVector3(this Vector2Int origin)
    {
        return new Vector3Int(origin.x, 0, origin.y);
    }
}