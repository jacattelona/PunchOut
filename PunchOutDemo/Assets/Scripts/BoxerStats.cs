using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerStats
{
    private int totalHits = 0;
    private int totalPunches = 0;

    private int totalOpponentPunches = 0;
    private int totalSuccessfulDodges = 0;
    private int totalDodges = 0;

    public void AddDodge()
    {
        totalDodges++;
    }

    public void AddPunch()
    {
        totalPunches++;
    }

    public void AddOpponentPunch()
    {
        totalOpponentPunches++;
    }

    public void AddSuccessfulDodge()
    {
        totalSuccessfulDodges++;
    }

    public void AddHit()
    {
        totalHits++;
    }

    /// <summary>
    /// Get the percent of hits landed
    /// </summary>
    /// <returns></returns>
    public float GetHitPercentage()
    {
        if (totalPunches == 0) return 0;
        return totalHits / (float)totalPunches * 100;
    }

    /// <summary>
    /// Get the percent of incoming punches dodged
    /// </summary>
    /// <returns></returns>
    public float GetDodgePercentage()
    {
        if (totalOpponentPunches == 0) return 0;
        return totalSuccessfulDodges / (float)totalOpponentPunches * 100;
    }

    /// <summary>
    /// Get the percent of dodges that successfully avoided a punch
    /// </summary>
    /// <returns></returns>
    public float GetSuccessfulDodgePercentage()
    {
        if (totalDodges == 0) return 0;
        return totalSuccessfulDodges / (float)totalDodges * 100;
    }

}
