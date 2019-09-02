using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyHeuristic : Decision
{
    private float lastPunch = 1f;

    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        if (lastPunch == 1f)
        {
            lastPunch = 2f;
        } else
        {
            lastPunch = 1f;
        }
        return new float[] { 0f, lastPunch };
    }

    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
