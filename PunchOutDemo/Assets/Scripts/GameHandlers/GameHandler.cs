using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{

    public float maxTime = 60f;

    public Transform cameraTransform;
    private Vector3 cameraVelocity = Vector3.zero;
    public Vector3 offenseLocation, defenseLocation, matchLocation;

    [SerializeField]
    public GameObject mainMenu, offensiveTraining, defensiveTraining, match;
    public Text timer;

    private const int STATE_MAIN_MENU = 0, STATE_OFFENSIVE = 1, STATE_DEFENSIVE = 3, STATE_MATCH = 4, STATE_TO_DEFENSIVE = 5, STATE_TO_MATCH = 6;

    private int state = STATE_MAIN_MENU;

    float timeLeft = 0;



    // Start is called before the first frame update
    void Start()
    {
        state = STATE_MAIN_MENU;
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Move between the phases
        switch (state)
        {
            case STATE_MAIN_MENU:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartOffensive();
                }
                break;

            case STATE_OFFENSIVE:
                TimeUpdate();
                if (timeLeft <= 0)
                    SwitchDefensive();
                break;

            case STATE_DEFENSIVE:
                TimeUpdate();
                if (timeLeft <= 0)
                    SwitchMatch();
                break;

            case STATE_MATCH:
                break;

            case STATE_TO_DEFENSIVE:
                if (MoveCameraToPosition(defenseLocation))
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        StartDefensive();
                    }
                    //StartDefensive();
                }
                break;
            case STATE_TO_MATCH:
                if (MoveCameraToPosition(matchLocation))
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        StartMatch();
                    }
                }
                    
                break;
        }
    }
    
    private bool MoveCameraToPosition(Vector3 position)
    {
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, position, ref cameraVelocity, .5f);
        return Vector3.Distance(cameraTransform.position, position) < .01f;
    }

    private void StartOffensive()
    {
        state = STATE_OFFENSIVE;
        timeLeft = maxTime;
        timer.text = "Time Left: " + (int)timeLeft;

        offensiveTraining.SetActive(true);
        defensiveTraining.SetActive(false);
        match.SetActive(false);
    }

    
    private void SwitchDefensive()
    {
        state = STATE_TO_DEFENSIVE;
        timeLeft = maxTime;
        timer.text = "Press Space to Start";

        offensiveTraining.SetActive(false);
        defensiveTraining.SetActive(true);
        match.SetActive(false);
    }

    private void StartDefensive()
    {
        state = STATE_DEFENSIVE;
        timer.text = "Time Left: " + (int)timeLeft;
        defensiveTraining.GetComponentInChildren<DefensiveTrainingMatchHandler>().BeginTraining();
    }


    private void SwitchMatch()
    {
        state = STATE_TO_MATCH;
        timeLeft = maxTime;
        timer.text = "Press Space to Start";

        offensiveTraining.SetActive(false);
        defensiveTraining.SetActive(false);
        match.SetActive(true);
        //match.GetComponent<Match>().StopFight();
        //match.transform.Find("Match").GetComponent<Match>().StopFight();
    }

    private void StartMatch()
    {
        state = STATE_MATCH;
        timer.text = "Fight!";
        //match.GetComponent<Match>().StartFight();
        match.GetComponentInChildren<MatchGameHandler>().BeginFight();
    }

    private void TimeUpdate()
    {
        timeLeft -= Time.deltaTime;
        timer.text = "Time Left: " + (int)timeLeft;
    }

}
