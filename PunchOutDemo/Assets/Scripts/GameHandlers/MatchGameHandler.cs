using TMPro;
using UnityEngine;

public class MatchGameHandler : MonoBehaviour
{

    [SerializeField]
    public Match match;

    [SerializeField]
    private GameObject waitingScreen;

    [SerializeField]
    private GameObject endingScreen;

    private string winner;

    private const int STATE_WAITING = 0, STATE_FIGHTING = 1, STATE_KO = 2, STATE_END = 3;

    private int state;

    private bool ready = false;

    private bool spoke = false;

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

                if (!spoke)
                {
                    CoachDialog.instance?.Show("Now sit back and watch Roboxer fight in the Machine Learning Arena!", 3.0f);
                    spoke = true;
                }
                endingScreen.SetActive(false);

                // When trigger condition
                waitingScreen.SetActive(true);
                if (ready)
                {
                    waitingScreen.SetActive(false);
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
                EndGame();
                break;
        }

    }

    private void OnBoxerKO()
    {

        if (match.GetPlayer1().IsKO())
        {
            winner = "The Opponent";
        } else
        {
            winner = "Roboxer";
        }

    }

    private void EndGame()
    {
        endingScreen.SetActive(true);
        endingScreen.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = winner + " won!";
    }



    public void BeginFight()
    {
        ready = true;
    }
}
