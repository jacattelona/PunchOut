using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public int max;
    public int health;

    // Start is called before the first frame update
    void Start()
    {
        health = max;
    }

    /// <summary>
    /// Get the current health
    /// </summary>
    /// <returns>The health</returns>
    public int GetHealth()
    {
        return health;
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
        this.health = Mathf.Max(Mathf.Min(health, max), 0);
    }

    /// <summary>
    /// Get the health percentage [0, 100]
    /// </summary>
    /// <returns>The health percentage</returns>
    public float GetHealthPercentage()
    {
        return health / (float)max * 100;
    }
}
