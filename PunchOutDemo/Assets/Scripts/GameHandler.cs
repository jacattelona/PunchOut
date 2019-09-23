using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{

    public GameObject offensiveTrainingCoachArea, offensiveTrainingAIArea;

    public float demoTime, viewTime;

    private const int STATE_DEMO = 0, STATE_VIEW = 1;

    private int state = STATE_DEMO;

    private float lastTime;

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

        lastTime = Time.fixedTime;
        imitationSystem.shouldImitate = true;
        rewardHistory.isRecording = true;
        SetAIEnabled(false);
        SetCoachEnabled(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case STATE_DEMO:
                if (Time.fixedTime - lastTime > demoTime)
                {
                    state = STATE_VIEW;
                    lastTime = Time.fixedTime;
                    imitationSystem.shouldImitate = false;
                    rewardHistory.isRecording = false;
                    SetAIEnabled(true);
                    SetCoachEnabled(false);
                }
                break;
            case STATE_VIEW:
                if (Time.fixedTime - lastTime > viewTime)
                {
                    state = STATE_DEMO;
                    lastTime = Time.fixedTime;
                    imitationSystem.shouldImitate = true;
                    rewardHistory.isRecording = true;
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
