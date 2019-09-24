using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingProgressReward : MonoBehaviour
{
    [SerializeField]
    private TrainingProgress trainingProgress;

    [SerializeField]
    private RewardHistory rewardHistory;

    private void Update()
    {
        if (rewardHistory.rewards.Count == 0) return;
        trainingProgress.value = rewardHistory.rewards[rewardHistory.rewards.Count - 1].Amount;
    }
}
