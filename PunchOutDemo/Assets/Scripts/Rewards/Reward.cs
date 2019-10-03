using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Reward: ScriptableObject
{
    public float punchReward;
    public float punchPenalty;
    public float dodgeReward;
    public float dodgePenalty;
    public float knockOutReward;
    public float knockOutPenalty;
    public float existancePenalty;
    public float imitationReward;
    public float imitationPenalty;
}
