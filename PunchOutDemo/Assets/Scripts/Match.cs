using MLAgents;
using UnityEngine;

public class Match : Area
{
    [SerializeField]
    public Boxer2 player1, player2;

    private bool isFighting;

    /// <summary>
    /// Start the fight
    /// </summary>
    public void StartFight()
    {
        if (isFighting) return;
        player1.isFighting = true;
        player2.isFighting = true;
        player1.SetOpponent(player2);
        player2.SetOpponent(player1);
        isFighting = true;
    }

    /// <summary>
    /// Stop the fight
    /// </summary>
    public void StopFight()
    {
        if (!isFighting) return;
        player1.isFighting = false;
        player2.isFighting = false;
        player1.SetOpponent(null);
        player2.SetOpponent(null);
        isFighting = false;
    }

    /// <summary>
    /// Determines if the players are fighting
    /// </summary>
    public bool IsFighting()
    {
        return isFighting;
    }

    /// <summary>
    /// Reset the boxers and the stop the fight
    /// </summary>
    public void ResetMatch()
    {
        StopFight();
        player1.ResetBoxer();
        player2.ResetBoxer();
    }

    public Boxer2 GetPlayer1()
    {
        return player1;
    }

    public Boxer2 GetPlayer2()
    {
        return player2;
    }
}
