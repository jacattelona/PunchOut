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
        var match = trainee.currentAction == coach.currentAction;//MLActionFactory.GetAction(trainee.lastActions) == MLActionFactory.GetAction(coach.lastActions);
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
    /// Get the Needleman-Wunsch score of the boxers' move histories
    /// </summary>
    /// <returns>The alignment score</returns>
    public float GetNeedlemanWunschScore()
    {
        const float GAP_PENALTY = -1f;
        const float MATCH_AWARD = 1f;
        const float MISMATCH_PENALTY = -1f;

        var coachActions = coach.actionHistory.GetActions();
        var traineeActions = trainee.actionHistory.GetActions();

        var n = coachActions.Count;
        var m = traineeActions.Count;

        var score = new float[m + 1, n + 1];

        for (var i = 0; i < m + 1; i++)
        {
            score[i, 0] = GAP_PENALTY * i;
        }

        for (var j = 0; j < n + 1; j++)
        {
            score[0, j] = GAP_PENALTY * j;
        }

        for (var i = 1; i < m + 1; i++)
        {
            for (var j = 1; j < n + 1; j++)
            {

                var first = coachActions[j - 1].action;
                var second = traineeActions[i - 1].action;
                float match = score[i - 1, j - 1] + (first == second ? MATCH_AWARD : MISMATCH_PENALTY);
                float delete = score[i - 1, j] + GAP_PENALTY;
                float insert = score[i, j - 1] + GAP_PENALTY;
                score[i, j] = Mathf.Max(match, delete, insert);
            }
        }

        return score[m, n];

    }


    /// <summary>
    /// Get the identify score of the two boxers (how many of their actions matched)
    /// </summary>
    /// <returns>A score between 0 and 1</returns>
    public float GetIdentityScore()
    {
        var coachActions = coach.actionHistory.GetActions();
        var traineeActions = trainee.actionHistory.GetActions();
        if (coachActions.Count == 0) return 0f;
        int minLength = Mathf.Min(coachActions.Count, traineeActions.Count);
        int matches = 0;
        for (var i = 0; i < minLength; i++)
        {
            if (coachActions[i].action == traineeActions[i].action)
            {
                matches++;
            }
        }
        return matches / (float) coachActions.Count;
    }

    /// <summary>
    /// Get the Dynamic Time Warping score of the two agents
    /// </summary>
    /// <returns>The DTW score</returns>
    public float GetDTWScore()
    {
        var coachActions = coach.actionHistory.GetActions();
        var traineeActions = trainee.actionHistory.GetActions();

        var n = coachActions.Count;
        var m = traineeActions.Count;

        var dtw = new float[n + 1, m + 1];
        for(var i = 1; i <= n; i++)
        {
            for(var j = 1; j <= m; j++)
            {
                dtw[i, j] = float.PositiveInfinity;
            }
        }
        dtw[0, 0] = 0;

        for (var i = 1; i <= n; i++)
        {
            for (var j = 1; j <= m; j++)
            {
                var cost = GetDTWDistance(coachActions[i - 1], traineeActions[j - 1]);
                dtw[i, j] = cost + Mathf.Min(dtw[i - 1, j],
                                             dtw[i, j - 1],
                                             dtw[i - 1, j - 1]);

            }
        }

        return dtw[n, m];
    }

    /// <summary>
    /// Get the Dynamic Time Warping score of the two agents (normalized)
    /// </summary>
    /// <returns>The normalized DTW score between 0 and 1, where 1 is 100% similar</returns>
    public float GetNormalizedDTWScore()
    {
        var coachActions = coach.actionHistory.GetActions();
        var traineeActions = trainee.actionHistory.GetActions();

        var n = coachActions.Count;
        var m = traineeActions.Count;

        if (n == 0 || m == 0) return 0;

        var dtw = GetDTWScore();

        var largestDistance = Mathf.Sqrt(1 + Mathf.Pow(Mathf.Max(m, n), 2));

        return Mathf.Clamp01((largestDistance - dtw) / largestDistance);
    }

    private float GetDTWDistance(ActionHistory.MLActionEvent e1, ActionHistory.MLActionEvent e2)
    {
        var MISMATCH_COST = 1f;
        var actionDelta = e1.action == e2.action ? 0f: MISMATCH_COST;

        var timeDelta = e2.time - e1.time;
        return Mathf.Sqrt(actionDelta * actionDelta + timeDelta * timeDelta);
    }
    

    /// <summary>
    /// Reset the evaluator
    /// </summary>
    public void Reset()
    {
        total = 0;
        matching = 0;
        coach.actionHistory = new ActionHistory();
        trainee.actionHistory = new ActionHistory();
    }

}
