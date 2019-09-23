using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{

    public GameObject offensiveTrainingCoachArea, offensiveTrainingAIArea;

    private BoxerArea area;

    public float demoTime, viewTime;

    private const int STATE_DEMO = 0, STATE_VIEW = 1;

    private int state = STATE_DEMO;

    private float lastCount;

    private ImitationSystem imitationSystem;
    private RewardHistory rewardHistory;
    private Renderer[] coachRenderers, aiRenderers;

    // Start is called before the first frame update
    void Start()
    {

        imitationSystem = offensiveTrainingAIArea.GetComponentInChildren<ImitationSystem>();
        coachRenderers = offensiveTrainingCoachArea.GetComponentsInChildren<Renderer>();
        aiRenderers = offensiveTrainingAIArea.GetComponentsInChildren<Renderer>();
        rewardHistory = offensiveTrainingAIArea.GetComponentInChildren<RewardHistory>();
        area = offensiveTrainingAIArea.GetComponent<BoxerArea>();

        lastCount = area.matchNumber;
        imitationSystem.shouldImitate = true;
        rewardHistory.isRecording = true;

        // DO SOMETHING BETTER WITH THE UI BELOW
        SetAIEnabled(false);
        SetCoachEnabled(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case STATE_DEMO:
                if (area.matchNumber - lastCount >= demoTime)
                {
                    state = STATE_VIEW;
                    lastCount = area.matchNumber;
                    imitationSystem.shouldImitate = false;
                    rewardHistory.isRecording = false;

                    // DO SOMETHING BETTER WITH THE UI BELOW
                    SetAIEnabled(true);
                    SetCoachEnabled(false);
                }
                break;
            case STATE_VIEW:
                if (area.matchNumber - lastCount >= viewTime)
                {
                    state = STATE_DEMO;
                    lastCount = area.matchNumber;
                    imitationSystem.shouldImitate = true;
                    rewardHistory.isRecording = true;

                    // DO SOMETHING BETTER WITH THE UI BELOW
                    SetAIEnabled(false);
                    SetCoachEnabled(true);
                }
                break;
        }
    }

    private void SetAIEnabled(bool enabled)
    {
        foreach (Renderer renderer in aiRenderers)
        {
            renderer.enabled = enabled;
        }
    }

    private void SetCoachEnabled(bool enabled)
    {
        foreach (Renderer renderer in coachRenderers)
        {
            renderer.enabled = enabled;
        }
    }
}
