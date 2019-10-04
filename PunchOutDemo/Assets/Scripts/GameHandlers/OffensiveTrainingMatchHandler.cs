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

    private const int STATE_WAITING = 0, STATE_DEMONSTRATING = 1, STATE_VIEW_AI = 2, STATE_END = 3;

    private int state;

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
                    state = STATE_VIEW_AI;
                }
                break;
            case STATE_VIEW_AI:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    demoStartTime = Time.time;
                    Demonstrate();
                    state = STATE_DEMONSTRATING;
                }
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
        coachMatch.StartFight();

        // Stop AI's match and look at the coach
        Boxer AI = aiMatches[0].GetPlayer1();
        aiMatches[0].StopFight();
        AI.transform.Find("Sprite").localEulerAngles = new Vector3(0, 0, 90);

        // Start the hidden AIs
        StartHiddenAIMatches();       
    }

    private void Watch()
    {

        // Stop coach's match and look at AI
        coachMatch.StopFight();
        coachMatch.GetPlayer1().transform.Find("Sprite").localEulerAngles = new Vector3(0, 0, -90);

        // Start the AI's match
        Boxer AI = aiMatches[0].GetPlayer1();
        aiMatches[0].StartFight();
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
