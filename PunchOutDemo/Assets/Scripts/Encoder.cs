using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encoder
{
    private Encoder() { }

    /// <summary>
    /// Encode a range-bounded integer as a one-hot float array
    /// </summary>
    /// <param name="value">The value</param>
    /// <param name="minValue">The minimim range</param>
    /// <param name="maxValue">The maximum range</param>
    /// <returns>The one-hot array</returns>
    public static float[] encodeInt(int value, int minValue, int maxValue)
    {
        int range = 1 + maxValue - minValue;
        int position = value - minValue;
        float[] encoded = new float[range];
        if (position < 0 || position >= range)
        {
            return encoded;
        }
        encoded[position] = 1;
        return encoded;
    }
}
