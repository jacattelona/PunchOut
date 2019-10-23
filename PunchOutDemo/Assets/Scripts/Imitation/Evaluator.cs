using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class to evaluate an imitation learning algorithm
/// </summary>
public class Evaluator: MonoBehaviour
{
    private int matching = 0;
    private int total = 0;

    [SerializeField]
    private Boxer trainee;

    [SerializeField]
    private Boxer coach;

    private void Update()
    {
        var match = MLActionFactory.GetAction(trainee.lastActions) == MLActionFactory.GetAction(coach.lastActions);
        AddSample(match);
    }


    /// <summary>
    /// Add a sample to the evaluator
    /// </summary>
    /// <param name="match">True if the boxer matches the coach, false otherwise</param>
    public void AddSample(bool match)
    {
        if (match) matching++;
        total++;
    }

    /// <summary>
    /// Get the current score of the algorithm
    /// </summary>
    /// <returns>A score between 0 and 1, where 1 is a complete match</returns>
    public float GetScore()
    {
        if (total == 0) return 0f;
        return matching / (float) total;
    }

    /// <summary>
    /// Reset the evaluator
    /// </summary>
    public void Reset()
    {
        total = 0;
        matching = 0;
    }

}
