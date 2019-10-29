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

    /// <summary>
    /// Calculate the cross entropy of a probability
    /// </summary>
    /// <param name="probability">The probability of the action taken</param>
    /// <param name="correct">True if the action was correct, false otherwise</param>
    /// <returns>The cross entropy of the action</returns>
    public static float CrossEntropy(float probability, bool correct)
    {
        if (correct)
        {
            return -Mathf.Log(probability);
        } else
        {
            return -Mathf.Log(1 - probability);
        }
    }

    /// <summary>
    /// Calculate the cross entropy of a probability in a multi-class scenario
    /// </summary>
    /// <param name="probability">The probability of the action taken</param>
    /// <param name="predictedClass">The predicted action</param>
    /// <param name="actualClass">The actual action</param>
    /// <returns>The cross entropy of the action</returns>
    public static float CrossEntropy(float probability, int predictedClass, int actualClass)
    {
        var multiplier = predictedClass == actualClass ? 1 : 0;
        return -multiplier * Mathf.Log(probability);
    }
}
