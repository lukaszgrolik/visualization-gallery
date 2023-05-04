using UnityEngine;
using System.Collections.Generic;

public static class FloatExtensions
{
    public static string ToPercentageString(this float val)
    {
        return Mathf.Round(val * 100) + "%";
    }

    public static bool NearlyEqual(this float self, float val, float precision = .001f)
    {
        return Mathf.Abs(self - val) < precision;
    }

    public static float RoundDecimal(this float self, int precision)
    {
        var val = Mathf.Pow(10, precision);

        return Mathf.Round(self * val) / val;
    }

    public static float RoundPrec(this float self, float prec)
    {
        if (prec <= 0) throw new System.Exception("precision must be greater than 0");

        return Mathf.Round(self / prec) * prec;
    }
}