using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
    private MathUtils() { }

    public static float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        float normal = Normalize(value, oldMin, oldMax);
        float range = newMax - newMin;
        float newValue = normal * range + newMin;
        return newValue;
    }

    public static float Map01(float value, float oldMin, float oldMax)
    {
        return Map(value, oldMin, oldMax, 0, 1);
    }

    public static float Normalize(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }
}
