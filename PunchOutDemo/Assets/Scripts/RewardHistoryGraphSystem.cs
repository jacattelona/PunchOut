using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardHistoryGraphSystem : MonoBehaviour
{

    public GraphData graphData;
    public float maxReward = 40;
    public float minReward = -40;

    private RewardHistory rewardHistory;


    // Start is called before the first frame update
    void Start()
    {
        rewardHistory = GetComponent<RewardHistory>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        graphData.data = RewardsToFloatList(rewardHistory.rewards);
    }

    private List<float> RewardsToFloatList(List<RewardHistory.Reward> rewards)
    {
        List<float> floats = new List<float>();
        foreach(RewardHistory.Reward reward in rewards)
        {
            floats.Add((reward.Amount - minReward) / (maxReward - minReward));
        }
        return floats;
    }
}
