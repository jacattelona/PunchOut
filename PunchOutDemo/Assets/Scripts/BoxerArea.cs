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

        playerBoxer.punchAction.action.AddListener(OpponentPunched);
        opponentBoxer.punchAction.action.AddListener(PlayerPunched);
        matchNumber = 1;
        matchNumberDisp.text = string.Format("Match {0}", matchNumber);
    }

    private void PlayerPunched(int side)
    {
        PunchOutcome outcome = playerBoxer.onPunched(opponentBoxer.GetPunchState());
        opponentBoxer.RewardOutcome(outcome);
    }

    private void OpponentPunched(int side)
    {
        PunchOutcome outcome = opponentBoxer.onPunched(playerBoxer.GetPunchState());
        playerBoxer.RewardOutcome(outcome);
    }

    public override void ResetArea()
    {
        base.ResetArea();
        matchNumber += 1f;
        matchNumberDisp.text = string.Format("Match {0}", matchNumber);
    }

}