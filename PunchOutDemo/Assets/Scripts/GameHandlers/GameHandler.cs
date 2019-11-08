using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{

    public float maxTime = 2f;

    public Transform cameraTransform;
    private Vector3 cameraVelocity = Vector3.zero;
    public Vector3 offenseLocation, defenseLocation, matchLocation;

    public TutorialManager tutorialManager;
    public GameObject tutorial;
    public GameObject timerOb;
    public GameObject matchTutorial;

    [SerializeField]
    public GameObject mainMenu, offensiveTraining, defensiveTraining, match;
    public Text timer;

    private const int STATE_MAIN_MENU = 0, STATE_OFFENSIVE = 1, STATE_DEFENSIVE = 3, STATE_MATCH = 4, STATE_TO_DEFENSIVE = 5, STATE_TO_MATCH = 6, STATE_TUTORIAL1 = 7, STATE_TUTORIAL2 = 8;

    private int state = STATE_MAIN_MENU;

    float timeLeft = 0;

    private float timeScale, fixedDt;



    // Start is called before the first frame update
    void Start()
    {
        tutorial = GameObject.Find("Animated");
        state = STATE_TUTORIAL1;
        timeScale = Time.timeScale;
        fixedDt = Time.fixedDeltaTime;
    }

    public void setState(int stateNew)
    {
        state = stateNew;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale != 0) // Pause
        {
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
        } else if (Time.timeScale == 0 && Input.GetKeyDown(KeyCode.Escape)) // Unpause
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = fixedDt;
        }

        // TODO: Move between the phases
        switch (state)
        {
            case STATE_MAIN_MENU:
                if (Input.GetKeyDown(KeyCode.Space) && tutorialManager.state == 5)
                {
                    StartOffensive();
                }
                break;

            case STATE_OFFENSIVE:
                TimeUpdate();
                if (timeLeft <= 0)
                    SwitchMatch();
                break;


            case STATE_TUTORIAL1:
                StartTutorial();
                break;

            case STATE_TUTORIAL2:
                StartTutorial2();
                break;

            /*case STATE_DEFENSIVE:
                TimeUpdate();
                if (timeLeft <= 0)
                    SwitchMatch();
                break;*/

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
                    matchTutorial.SetActive(false);
                    match.SetActive(true);
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

    private void StartTutorial()
    {
        timer.text = " ";
        offensiveTraining.SetActive(false);
        defensiveTraining.SetActive(false);
        match.SetActive(false);
        matchTutorial.SetActive(false);
    }

    private void StartTutorial2()
    {
        timer.text = " ";
        
        offensiveTraining.SetActive(false);
        defensiveTraining.SetActive(false);
        match.SetActive(false);
    }

    public void StartOffensive()
    {
        state = STATE_OFFENSIVE;
        timeLeft = 120;
        timer.text = "Time Left: " + (int)timeLeft;
        tutorial.SetActive(false);
        offensiveTraining.SetActive(true);
        defensiveTraining.SetActive(false);
        match.SetActive(false);
        matchTutorial.SetActive(false);
    }

    
    private void SwitchDefensive()
    {
        state = STATE_TO_DEFENSIVE;
        timeLeft = maxTime;
        timer.text = "Press Space to Start";

        offensiveTraining.SetActive(false);
        defensiveTraining.SetActive(true);
        match.SetActive(false);
        matchTutorial.SetActive(false);
    }

    private void StartDefensive()
    {
        state = STATE_DEFENSIVE;
        timer.text = "Time Left: " + (int)timeLeft;
        defensiveTraining.GetComponentInChildren<DefensiveTrainingMatchHandler>().BeginTraining();
    }


    private void SwitchMatch()
    {
        state = STATE_TUTORIAL2;

        timeLeft = maxTime;
        timer.text = "Press Space to Start";
        matchTutorial.SetActive(true);
        //tutorial.SetActive(true);
        //tutorialManager.state = 9;
        //tutorialManager.anim.SetTrigger("gotoExplainTourney");
        offensiveTraining.SetActive(false);
        defensiveTraining.SetActive(false);
        match.SetActive(false);
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
