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
        playerBoxer.onPunched(opponentBoxer.GetPunchState());
    }

    private void OpponentPunched()
    {
        opponentBoxer.onPunched(playerBoxer.GetPunchState());
    }

}