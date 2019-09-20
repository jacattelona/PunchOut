using System.Collections.Generic;
using UnityEngine;

public class RewardHistory : MonoBehaviour
{

    public struct Reward
    {
        public float Amount;
        public float Time;
    }

    public List<Reward> rewards = new List<Reward>();
}
