using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public Transform camTransform;
    public Vector3 coachCameraPos, aiCameraPos;
    private Vector3 velocity = Vector3.zero;

    public GameObject offensiveTrainingCoachArea, offensiveTrainingAIArea;

    private Vector3 coachAreaPos, aiAreaPos;

    private BoxerArea area;

    public float demoTime, viewTime;

    private const int STATE_DEMO = 0, STATE_VIEW = 1, STATE_SWITCH_DEMO = 3, STATE_SWITCH_VIEW = 4;

    private int state = STATE_SWITCH_DEMO;

    private float lastCount;

    private ImitationSystem[] imitationSystems;
    private List<RewardHistory> rewardHistories;

    //public enum CameraState
    //{
    //    Coach,
    //    Agent,
    //    SwitchingToCoach,
    //    SwitchingToAgent
    //}

    //public CameraState camState;
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
        if (Input.GetKeyDown(KeyCode.Alpha9))
            state = STATE_SWITCH_DEMO;
        if (Input.GetKeyDown(KeyCode.Alpha0))
            state = STATE_SWITCH_VIEW;

        switch (state)
        {
            case STATE_DEMO:
                SetAIEnabled(false);
                SetCoachEnabled(true);
                if (area.matchNumber - lastCount >= demoTime)
                {
                    state = STATE_SWITCH_VIEW;
                    lastCount = area.matchNumber;
                }
                break;
            case STATE_VIEW:
                SetAIEnabled(true);
                SetCoachEnabled(false);
                if (area.matchNumber - lastCount >= viewTime)
                {
                    state = STATE_SWITCH_DEMO;
                    lastCount = area.matchNumber;
                }
                break;
            case STATE_SWITCH_DEMO:
                camTransform.position = Vector3.SmoothDamp(camTransform.position, coachCameraPos, ref velocity, .5f);
                if (Vector3.Distance(transform.position, coachCameraPos) < .01f)
                {
                    state = STATE_DEMO;
                }
                break;
            case STATE_SWITCH_VIEW:
                camTransform.position = Vector3.SmoothDamp(camTransform.position, aiCameraPos, ref velocity, .5f);
                if (Vector3.Distance(transform.position, aiCameraPos) < .01f)
                {
                    state = STATE_VIEW;
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
            //offensiveTrainingAIArea.transform.localPosition = new Vector3(0, 100, 0);
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
            //offensiveTrainingCoachArea.transform.localPosition = new Vector3(0, 100, 0);
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
