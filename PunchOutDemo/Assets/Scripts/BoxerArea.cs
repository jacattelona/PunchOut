using MLAgents;
using UnityEngine;
using UnityEngine.UI;

public class BoxerArea : Area
{

    public GameObject player;
    public GameObject opponent;
    public Text matchNumberDisp;

    public Boxer playerBoxer;
    public Boxer opponentBoxer;

    private float matchNumber;

    void Start()
    {
        playerBoxer = player.GetComponent<Boxer>();
        opponentBoxer = opponent.GetComponent<Boxer>();

        playerBoxer.punch.AddListener(OpponentPunched);
        opponentBoxer.punch.AddListener(PlayerPunched);
        matchNumber = 1;
        matchNumberDisp.text = string.Format("Match {0}", matchNumber);
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
        matchNumber += 0.5f;
        matchNumberDisp.text = string.Format("Match {0}", matchNumber);
    }

}