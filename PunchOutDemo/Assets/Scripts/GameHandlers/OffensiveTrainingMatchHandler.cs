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

    private int currentTrainingProgressIdx = 0;

    public float minReward, maxReward;

    public float trainTime;

    private float startTime, demoStartTime;

    private const int STATE_WAITING = 0, STATE_DEMONSTRATING = 1, STATE_MOVING_COACH = 2, STATE_MOVING_AI = 3, STATE_VIEW_AI = 4, STATE_END = 5;

    private int state;

    private float lerpProgress = 0;

    // Start is called before the first frame update
    void Start()
    {
        state = STATE_WAITING;
    }

    // Update is called once per frame
    void Update()
    {
        bool shouldTrain = Time.time - startTime >= trainTime;

        if (shouldTrain)
        {
            startTime = Time.time;
        }

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
                waitingScreen.SetActive(true);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    waitingScreen.SetActive(false);
                    Demonstrate();
                    coachMatch.StartFight();
                    demoStartTime = Time.time;
                    startTime = Time.time;
                    state = STATE_DEMONSTRATING;
                }
                break;
            case STATE_DEMONSTRATING:
                if (shouldTrain && Time.time - demoStartTime >= trainTime) // Train if the train time has been complete since switching to demo mode
                {
                    TrainAIs();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Watch();
                    lerpProgress = 0;
                    state = STATE_MOVING_AI;
                }
                break;
            case STATE_MOVING_AI:
                if (lerpProgress > 1)
                {
                    aiMatches[0].StartFight();
                    state = STATE_VIEW_AI;
                }
                MoveTowardsWatchPosition(lerpProgress);
                lerpProgress += 0.1f;
                break;
            case STATE_VIEW_AI:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Demonstrate();
                    lerpProgress = 0;
                    state = STATE_MOVING_COACH;
                }
                break;
            case STATE_MOVING_COACH:
                if (lerpProgress > 1)
                {
                    demoStartTime = Time.time;
                    coachMatch.StartFight();
                    state = STATE_DEMONSTRATING;
                }
                MoveTowardsDemoPosition(lerpProgress);
                lerpProgress += 0.1f;
                break;
            case STATE_END:
                SwitchToNextScene();
                break;
        }

    }

    private void Demonstrate()
    {
        // Start coach's match 
        coachMatch.GetPlayer1().transform.Find("Sprite").localEulerAngles = Vector3.zero;
        coachMatch.GetPlayer1().transform.localPosition = new Vector3(-3, 0);

        // Stop AI's match and look at the coach
        Boxer AI = aiMatches[0].GetPlayer1();
        aiMatches[0].StopFight();
        AI.transform.Find("Sprite").localEulerAngles = new Vector3(0, 0, 90);
        AI.transform.localPosition = new Vector3(3, 2);

        // Start the hidden AIs
        StartHiddenAIMatches();       
    }

    private void MoveTowardsWatchPosition(float lerp)
    {
        Boxer AI = aiMatches[0].GetPlayer1();
        var aiX = Mathf.Lerp(3, -3, lerp);
        var aiY = Mathf.Lerp(2, 0, lerp);
        AI.transform.localPosition = new Vector3(aiX, aiY);

        Boxer coach = coachMatch.GetPlayer1();
        var coachX = Mathf.Lerp(-3, 3, lerp);
        var coachY = Mathf.Lerp(0, 2, lerp);
        coach.transform.localPosition = new Vector3(coachX, coachY);
    }

    private void MoveTowardsDemoPosition(float lerp)
    {
        Boxer AI = aiMatches[0].GetPlayer1();
        var aiX = Mathf.Lerp(-3, 3, lerp);
        var aiY = Mathf.Lerp(0, 2, lerp);
        AI.transform.localPosition = new Vector3(aiX, aiY);

        Boxer coach = coachMatch.GetPlayer1();
        var coachX = Mathf.Lerp(3, -3, lerp);
        var coachY = Mathf.Lerp(2, 0, lerp);
        coach.transform.localPosition = new Vector3(coachX, coachY);
    }

    private void Watch()
    {

        // Stop coach's match and look at AI
        coachMatch.StopFight();
        coachMatch.GetPlayer1().transform.Find("Sprite").localEulerAngles = new Vector3(0, 0, 90);

        // Start the AI's match
        Boxer AI = aiMatches[0].GetPlayer1();
        AI.transform.Find("Sprite").localEulerAngles = Vector3.zero;

        // Stop the hidden matches
        StopHiddenAIMatches();        
    }

    private void TrainAIs()
    {
        float reward = GetAveragePerformanceScore();
        foreach (TrainingProgress progress in trainingProgress)
        {
            progress.SetProgress(MathUtils.Map01(reward, minReward, maxReward));
        }
        
        Train(coachMatch);
        foreach (Match match in aiMatches)
        {
            Train(match);
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
