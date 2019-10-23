using UnityEngine;

public class MatchGameHandler : MonoBehaviour
{

    [SerializeField]
    public Match match;

    private string winner;

    private const int STATE_WAITING = 0, STATE_FIGHTING = 1, STATE_KO = 2, STATE_END = 3;

    private int state;

    private bool ready = false;

    // Start is called before the first frame update
    void Start()
    {
        state = STATE_WAITING;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE_WAITING:
                // When trigger condition
                if (ready)
                {
                    match.StartFight();
                    state = STATE_FIGHTING;
                }
                break;
            case STATE_FIGHTING:
                if (match.GetPlayer1().IsKO() || match.GetPlayer2().IsKO())
                {
                    OnBoxerKO();
                    match.StopFight();
                    state = STATE_KO;
                }
                // If time out
                break;
            case STATE_KO:
                // Wait X seconds
                Debug.Log(winner + " won!");
                state = STATE_END;
                // If time out
                break;
            case STATE_END:
                // Maybe wait a little
                EndGame();
                break;
        }

    }

    private void OnBoxerKO()
    {

        if (match.GetPlayer1().IsKO())
        {
            winner = "Opponent";
        } else
        {
            winner = "AI";
        }

    }

    private void EndGame()
    {
        // TODO: Do something here
    }



    public void BeginFight()
    {
        ready = true;
    }
}
