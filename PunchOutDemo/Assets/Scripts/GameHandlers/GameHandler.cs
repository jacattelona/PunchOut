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

    [SerializeField]
    public GameObject offensiveTraining, match;

    private const int STATE_MAIN_MENU = 0, STATE_OFFENSIVE = 1, STATE_MATCH = 4, STATE_TO_MATCH = 6, STATE_TUTORIAL1 = 7, STATE_TUTORIAL2 = 8;

    private int state = STATE_MAIN_MENU;

    float timeLeft = 0;

    private float timeScale, fixedDt;



    // Start is called before the first frame update
    void Start()
    {
        state = STATE_TUTORIAL1;
        timeScale = Time.timeScale;
        fixedDt = Time.fixedDeltaTime;
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
                break;

            case STATE_OFFENSIVE:
                if (offensiveTraining.GetComponentInChildren<OffensiveTrainingMatchHandler>().IsDone())
                {
                    SwitchMatch();
                }
                break;


            case STATE_TUTORIAL1:
                StartTutorial();
                if (tutorialManager.IsDone())
                {
                    StartOffensive();
                }
                break;

            case STATE_TUTORIAL2:
                StartTutorial2();
                break;

            case STATE_MATCH:
                break;

            case STATE_TO_MATCH:
                if (MoveCameraToPosition(matchLocation))
                {
                    //matchTutorial.SetActive(false);
                    offensiveTraining.SetActive(false);
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
        tutorial.SetActive(true);
        offensiveTraining.SetActive(false);
        match.SetActive(false);
    }

    private void StartTutorial2()
    {        
        offensiveTraining.SetActive(false);
        match.SetActive(false);
    }

    public void StartOffensive()
    {
        state = STATE_OFFENSIVE;
        timeLeft = 10;
        tutorial.SetActive(false);
        offensiveTraining.SetActive(true);
        match.SetActive(false);
    }



    private void SwitchMatch()
    {
        state = STATE_TO_MATCH;

        timeLeft = maxTime;
        //matchTutorial.SetActive(true);
        //tutorial.SetActive(true);
        //tutorialManager.state = 9;
        //tutorialManager.anim.SetTrigger("gotoExplainTourney");
        //offensiveTraining.SetActive(false);
        match.SetActive(false);
        //match.GetComponent<Match>().StopFight();
        //match.transform.Find("Match").GetComponent<Match>().StopFight();
    }

    private void StartMatch()
    {
        match.SetActive(true);
        state = STATE_MATCH;
        
        //match.GetComponent<Match>().StartFight();
        match.GetComponentInChildren<MatchGameHandler>().BeginFight();
    }


}
