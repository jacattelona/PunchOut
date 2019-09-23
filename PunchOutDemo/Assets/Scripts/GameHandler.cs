using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{

    public GameObject offensiveTrainingCoachArea, offensiveTrainingAIArea;

    private Vector3 coachAreaPos, aiAreaPos;

    private BoxerArea area;

    public float demoTime, viewTime;

    private const int STATE_DEMO = 0, STATE_VIEW = 1;

    private int state = STATE_DEMO;

    private float lastCount;

    private ImitationSystem[] imitationSystems;
    private List<RewardHistory> rewardHistories;

    // Start is called before the first frame update
    void Start()
    {
        aiAreaPos = offensiveTrainingAIArea.transform.localPosition;
        coachAreaPos = offensiveTrainingCoachArea.transform.localPosition;

        imitationSystems = transform.parent.GetComponentsInChildren<ImitationSystem>();

        RewardHistory[] allRewards = transform.parent.GetComponentsInChildren<RewardHistory>();

        rewardHistories = new List<RewardHistory>();

        foreach (RewardHistory reward in allRewards)
        {
            if (reward.isRecording)
            {
                rewardHistories.Add(reward);
            }
        }



        area = offensiveTrainingAIArea.GetComponent<BoxerArea>();

        lastCount = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case STATE_DEMO:
                SetAIEnabled(false);
                SetCoachEnabled(true);
                if (area.matchNumber - lastCount >= demoTime)
                {
                    state = STATE_VIEW;
                    lastCount = area.matchNumber;
                }
                break;
            case STATE_VIEW:
                SetAIEnabled(true);
                SetCoachEnabled(false);
                if (area.matchNumber - lastCount >= viewTime)
                {
                    state = STATE_DEMO;
                    lastCount = area.matchNumber;
                }
                break;
        }
    }

    private void SetAIEnabled(bool enabled)
    {
        if (enabled)
        {
            offensiveTrainingAIArea.transform.localPosition = aiAreaPos;
        } else
        {
            offensiveTrainingAIArea.transform.localPosition = new Vector3(0, 100, 0);
        }
    }

    private void SetCoachEnabled(bool enabled)
    {
        if (enabled)
        {
            offensiveTrainingCoachArea.transform.localPosition = coachAreaPos;
        }
        else
        {
            offensiveTrainingCoachArea.transform.localPosition = new Vector3(0, 100, 0);
        }

        foreach (RewardHistory reward in rewardHistories)
        {
            reward.isRecording = enabled;
        }
        
        foreach (ImitationSystem imitation in imitationSystems)
        {
            imitation.shouldImitate = enabled;
        }
    }
}
