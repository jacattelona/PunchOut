using MLAgents;
using UnityEngine;

public class BoxerArea : Area
{

    public GameObject player;
    public GameObject opponent;

    public Boxer playerBoxer;
    public Boxer opponentBoxer;

    void Start()
    {
        playerBoxer = player.GetComponent<Boxer>();
        opponentBoxer = opponent.GetComponent<Boxer>();

        playerBoxer.punch.AddListener(OpponentPunched);
        opponentBoxer.punch.AddListener(PlayerPunched);
    }

    private void PlayerPunched()
    {
        PunchOutcome outcome = playerBoxer.onPunched(opponentBoxer.GetPunchState());
        playerBoxer.RewardOutcome(outcome);
        if (outcome == PunchOutcome.KO)
        {
            ResetArea();
        }
    }

    private void OpponentPunched()
    {
        PunchOutcome outcome = opponentBoxer.onPunched(playerBoxer.GetPunchState());
        opponentBoxer.RewardOutcome(outcome);
        if (outcome == PunchOutcome.KO)
        {
            ResetArea();
        }
    }

    public override void ResetArea()
    {
        base.ResetArea();
        playerBoxer.AgentReset();
        opponentBoxer.AgentReset();
    }

}