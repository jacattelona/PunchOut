using MLAgents;
using UnityEngine;

public class Match : Area
{
    [SerializeField]
    public Boxer player1, player2;

    private bool isFighting;

    /// <summary>
    /// Start the fight
    /// </summary>
    public void StartFight()
    {
        if (isFighting) return;
        player1.punchAction.action.AddListener(Player2Punched);
        player2.punchAction.action.AddListener(Player1Punched);
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
        player1.punchAction.action.RemoveListener(Player2Punched);
        player2.punchAction.action.RemoveListener(Player1Punched);
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

    public Boxer GetPlayer1()
    {
        return player1;
    }

    public Boxer GetPlayer2()
    {
        return player2;
    }

    private void Player1Punched(Direction side)
    {
        PunchOutcome outcome = player1.onPunched(player2.currentAction, player2.GetStrength());
        player2.RewardOutcome(outcome);
    }

    private void Player2Punched(Direction side)
    {
        PunchOutcome outcome = player2.onPunched(player1.GetCurrentAction(), player1.GetStrength());
        player1.RewardOutcome(outcome);
    }

}
