using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardIndicator : MonoBehaviour
{
    public ImitationSystem visibleAI, invisibleAI;
    float rewardsActive = 0;

    // Start is called before the first frame update
    void Start()
    {
        visibleAI.rewardEvent.AddListener(TrackReward);
        invisibleAI.rewardEvent.AddListener(TrackReward);
    }

    // Update is called once per frame
    void Update()
    {
        if (rewardsActive >= 0)
            rewardsActive -= Time.deltaTime*3f;

        float scale = rewardsActive * .1f + 1.0f;
        if (scale >= 3.0f) scale = 3.0f;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void TrackReward()
    {
        rewardsActive++;
    }
}
