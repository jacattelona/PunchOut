﻿using MLAgents;
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
        if (outcome == PunchOutcome.KO)
        {
            Debug.Log(playerBoxer.GetCumulativeReward());
            ResetArea();
        }
    }

    private void OpponentPunched(int side)
    {
        PunchOutcome outcome = opponentBoxer.onPunched(playerBoxer.GetPunchState());
        playerBoxer.RewardOutcome(outcome);
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
        matchNumber += 1f;
        matchNumberDisp.text = string.Format("Match {0}", matchNumber);
    }

}