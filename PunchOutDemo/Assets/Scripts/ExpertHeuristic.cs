using MLAgents;
using System.Collections.Generic;
using UnityEngine;

public class ExpertHeuristic : Decision
{

    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        if (vectorObs[10] == 1)
        {
            return new float[] { 1f, 0f };
        }

        if (vectorObs[11] == 1)
        {
            return new float[] { 2f, 0f };
        }

        if (vectorObs[13] == 1)
        {
            return new float[] { 0f, 2f };
        }
        
        return new float[] { 0f, 0f };
    }
    
    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
