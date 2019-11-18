using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A class to evaluate an imitation learning algorithm
/// </summary>
public class Evaluator: MonoBehaviour
{
    private int matching = 0;
    private int total = 0;

    private float crossEntropy = 0;
    private float runningAverageCorrectness = 0f;
    private float correctness = 0;

    public UnityEvent matchingEvent = new UnityEvent();

    [SerializeField]
    private Boxer trainee;

    [SerializeField]
    private Boxer coach;

    private void Update()
    {
        var match = trainee.currentAction == coach.currentAction;//MLActionFactory.GetAction(trainee.lastActions) == MLActionFactory.GetAction(coach.lastActions);
        if (match) matchingEvent.Invoke();
        var desiredAction = MLActionFactory.GetAction(coach.lastActions);
        var probability = MLActionFactory.GetProbabilityFromVector(desiredAction, trainee.lastActions);
        crossEntropy += MathUtils.CrossEntropy(probability);
        //correctness += probability;
        var alpha = 0.995f;
        runningAverageCorrectness = alpha * runningAverageCorrectness + (1 - alpha) * probability;
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
    public float GetMatchingScore()
    {
        if (total == 0) return 0f;
        return matching / (float) total;
    }

    /// <summary>
    /// Get the average cross entropy
    /// </summary>
    /// <returns>The cross entropy</returns>
    public float GetCrossEntropy()
    {
        if (total == 0) return 0f;
        return crossEntropy / total;
    }

    /// <summary>
    /// Get the running average cross entropy
    /// </summary>
    /// <returns>The running average cross entropy</returns>
    public float GetCorrectness()
    {
        return runningAverageCorrectness;
    }
    
    

    /// <summary>
    /// Reset the evaluator
    /// </summary>
    public void Reset()
    {
        total = 0;
        matching = 0;
        crossEntropy = 0;
    }

}
