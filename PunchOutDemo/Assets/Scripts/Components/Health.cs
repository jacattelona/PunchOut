using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int max;

    private int current;

    // Start is called before the first frame update
    void Start()
    {
        current = max;
    }

    /// <summary>
    /// Get the current health
    /// </summary>
    /// <returns>The health</returns>
    public int GetHealth()
    {
        return current;
    }

    /// <summary>
    /// Get the maximum health
    /// </summary>
    /// <returns>The max health</returns>
    public int GetMaxHealth()
    {
        return max;
    }

    /// <summary>
    /// Set the current health
    /// </summary>
    /// <param name="health">The health</param>
    public void SetHealth(int health)
    {
        current = Mathf.Max(Mathf.Min(health, max), 0);
    }

    /// <summary>
    /// Get the health percentage [0, 100]
    /// </summary>
    /// <returns>The health percentage</returns>
    public float GetHealthPercentage()
    {
        return current / (float)max * 100;
    }
}
