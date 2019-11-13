using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveTrainingMatchHandler : MonoBehaviour
{

    [SerializeField]
    public GameObject waitingScreen;

    [SerializeField]
    public Match coachMatch;

    [SerializeField]
    public List<Match> aiMatches;

    [SerializeField]
    public List<TrainingProgress> trainingProgress;


    [SerializeField]
    public bool displayEvaluationData = false;

    [SerializeField]
    public Timer trainingTimer;

    [SerializeField]
    public matchtutorialManage selectScreen;

    private Evaluator evaluator;
    private int cycles = 0;

    private int currentTrainingProgressIdx = 0;

    public float trainTime;

    private float viewTimer;

    private float lastUpdateTime = 0;

    private const int STATE_WAITING = 0, STATE_TRAIN = 1, STATE_MOVING_COACH = 2, STATE_MOVING_AI = 3, STATE_VIEW_AI = 4, STATE_CHOOSE_NEXT = 5, STATE_END = 6;

    private int state;

    private float lerpProgress = 0;

    // Start is called before the first frame update
    void Start()
    {
        state = STATE_WAITING;
        evaluator = transform.parent.GetComponentInChildren<Evaluator>();
    }

    // Update is called once per frame
    void Update()
    {
        ReviveKOBoxers();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowNextTrainingProgress();
        } else
        {
            ShowCurrentTrainingProgress();
        }

        // TODO: Determine when to switch to STATE_END

        switch (state)
        {
            case STATE_WAITING:
                // The initial waiting screen
                waitingScreen.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    waitingScreen.SetActive(false);
                    SwitchToTrainingState();
                }
                break;
            case STATE_TRAIN:
                // Update the training progress indicators
                UpdateTrainingProgress();

                if (Time.time - lastUpdateTime >= 11.5)
                {
                    evaluator.Reset();
                    lastUpdateTime = Time.time;
                }

                // Check to see if the training phase is over
                if (trainingTimer.IsExpired())
                {
                    Watch();
                    lerpProgress = 0;
                    CoachDialog.instance?.Show("Coach: Let's see how much of that you picked up.");
                    state = STATE_MOVING_AI;
                }
                break;
            case STATE_MOVING_AI:
                if (lerpProgress > 1)
                {
                    aiMatches[0].StartFight();
                    viewTimer = trainTime;
                    state = STATE_VIEW_AI;
                }
                MoveTowardsWatchPosition(lerpProgress);
                lerpProgress += 0.1f;
                break;
            case STATE_VIEW_AI:
                // Watch the AI fight for 1 cycle
                viewTimer -= Time.deltaTime;
                if (viewTimer <= 0)
                {
                    CoachDialog.instance?.Hide();
                    coachMatch.StopFight();
                    foreach (var match in aiMatches)
                    {
                        match.StopFight();
                    }
                    state = STATE_CHOOSE_NEXT;
                }
                break;
            case STATE_CHOOSE_NEXT:
                selectScreen.gameObject.SetActive(true);
                // TODO: Replace this with the 
                selectScreen.match.AddListener(() =>
                {
                    state = STATE_END;
                    selectScreen.gameObject.SetActive(false);
                });

                selectScreen.retrain.AddListener(() =>
                {
                    Demonstrate();
                    lerpProgress = 0;
                    state = STATE_MOVING_COACH;
                    selectScreen.gameObject.SetActive(false);
                });
                break;
            case STATE_MOVING_COACH:
                if (lerpProgress > 1)
                {
                    SwitchToTrainingState();
                }
                MoveTowardsDemoPosition(lerpProgress);
                lerpProgress += 0.1f;
                break;
            case STATE_END:
                break;
        }

    }


    private void SwitchToTrainingState()
    {
        CoachDialog.instance?.Show("Coach: AI, let me show you how to fight.", 2.0f);

        // Stop the AI matches, move everyone to the correct location
        Demonstrate();

        // Start the coach's match
        coachMatch.StartFight();

        // Start the countdown timer
        trainingTimer.ResetTimer();
        trainingTimer.StartTimer();

        // Reset the evaluation metrics
        evaluator.Reset();
        RewardIndicator r = transform.parent.GetComponentInChildren<RewardIndicator>();
        r.Activate();

        // Switch to training
        state = STATE_TRAIN;
    }


    public bool IsDone()
    {
        return state == STATE_END;
    }


    private void Demonstrate()
    {
        // Start coach's match 
        coachMatch.GetPlayer1().transform.localEulerAngles = Vector3.zero;
        coachMatch.GetPlayer1().transform.localPosition = new Vector3(0, 0);

        // Stop AI's match and look at the coach
        Boxer AI = aiMatches[0].GetPlayer1();
        aiMatches[0].StopFight();
        AI.transform.localEulerAngles = new Vector3(0, 0, -90);
        AI.transform.localPosition = new Vector3(-9.24f, -3.61f);

        // Start the hidden AIs
        StartHiddenAIMatches();       
    }

    private void MoveTowardsWatchPosition(float lerp)
    {
        Boxer AI = aiMatches[0].GetPlayer1();
        var aiX = Mathf.Lerp(-9.24f, 0, lerp);
        var aiY = Mathf.Lerp(-3.61f, 0, lerp);
        AI.transform.localPosition = new Vector3(aiX, aiY);

        Boxer coach = coachMatch.GetPlayer1();
        var coachX = Mathf.Lerp(0, -9.24f, lerp);
        var coachY = Mathf.Lerp(0, -3.61f, lerp);
        coach.transform.localPosition = new Vector3(coachX, coachY);
    }

    private void MoveTowardsDemoPosition(float lerp)
    {
        Boxer AI = aiMatches[0].GetPlayer1();
        var aiX = Mathf.Lerp(0, -9.24f, lerp);
        var aiY = Mathf.Lerp(0, -3.61f, lerp);
        AI.transform.localPosition = new Vector3(aiX, aiY);

        Boxer coach = coachMatch.GetPlayer1();
        var coachX = Mathf.Lerp(-9.24f, 0, lerp);
        var coachY = Mathf.Lerp(-3.61f, 0, lerp);
        coach.transform.localPosition = new Vector3(coachX, coachY);
    }

    private void Watch()
    {

        // Stop coach's match and look at AI
        coachMatch.StopFight();
        coachMatch.GetPlayer1().transform.localEulerAngles = new Vector3(0, 0, -90);

        // Start the AI's match
        Boxer AI = aiMatches[0].GetPlayer1();
        AI.transform.localEulerAngles = Vector3.zero;

        // Stop the hidden matches
        StopHiddenAIMatches();        
    }

    private void TrainAIs()
    {
        UpdateTrainingProgress();
        
        Train(coachMatch);
        foreach (Match match in aiMatches)
        {
            Train(match);
        }
    }

    private void UpdateTrainingProgress()
    {
        float p = evaluator.GetCorrectness();
        foreach (TrainingProgress progress in trainingProgress)
        {
            progress.SetProgress(p);
        }
    }

    private void ShowCurrentTrainingProgress()
    {
        for (int i = 0; i < trainingProgress.Count; i++)
        {
            trainingProgress[i].SetEnabled(true);
        }
    }

    private void ShowNextTrainingProgress()
    {
        currentTrainingProgressIdx++;
        currentTrainingProgressIdx = currentTrainingProgressIdx % trainingProgress.Count;
        ShowCurrentTrainingProgress();
    }

    private float GetAveragePerformanceScore()
    {
        if (aiMatches.Count == 0) return 0;
        float sum = 0;
        for(int i = 1; i < aiMatches.Count; i++)
        {
            float score = aiMatches[i].GetPlayer1().GetPerformanceScore();
            sum += score;
        }
        return sum / aiMatches.Count;
    }

    private void Train(Match match)
    {
        match.GetPlayer1().Train();
        match.GetPlayer2().Train();
    }

    private void StartHiddenAIMatches()
    {
        for(int i = 1; i < aiMatches.Count; i++)
        {
            aiMatches[i].StartFight();
        }
    }

    private void StopHiddenAIMatches()
    {
        for (int i = 1; i < aiMatches.Count; i++)
        {
            aiMatches[i].StopFight();
        }
    }

    private void ReviveKOBoxers()
    {
        Revive(coachMatch);
        foreach (Match match in aiMatches)
        {
            Revive(match);
        }
    }

    private void Revive(Match match)
    {
        if (match.GetPlayer1().IsKO())
        {
            match.GetPlayer1().Revive();
        }

        if (match.GetPlayer2().IsKO())
        {
            match.GetPlayer2().Revive();
        }
    }

    private void SwitchToNextScene()
    {
        // TODO: Do something here
    }
}
