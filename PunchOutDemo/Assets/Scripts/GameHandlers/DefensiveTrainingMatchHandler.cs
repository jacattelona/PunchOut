using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefensiveTrainingMatchHandler : MonoBehaviour
{

    [SerializeField]
    public Match match;

    [SerializeField]
    public Boxer aiBoxer;

    [SerializeField]
    public List<TrainingProgress> trainingProgress;

    public float minScore, maxScore;

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
                    state = STATE_KO;
                }
                // If time out
                break;
            case STATE_KO:
                // Wait X seconds
                match.StartFight();
                state = STATE_FIGHTING;
                // If time out
                break;
            case STATE_END:
                // Maybe wait a little
                SwitchToNextScene();
                break;
        }
        
    }

    private void OnBoxerKO()
    {
        // Notify of KO
        Debug.Log(aiBoxer.GetPerformanceScore());
        var performance = MathUtils.Map01(aiBoxer.GetPerformanceScore(), minScore, maxScore);
        SetProgress(performance);
        aiBoxer.Train();
        match.ResetMatch();
    }

    private void SetProgress(float progress)
    {
        foreach (var tp in trainingProgress)
        {
            tp.SetProgress(progress);
        }
    }

    private void SwitchToNextScene()
    {
        // TODO: Do something here
    }

    public void BeginTraining()
    {
        ready = true;
    }
}
