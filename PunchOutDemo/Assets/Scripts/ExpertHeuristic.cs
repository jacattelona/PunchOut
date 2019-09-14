using MLAgents;
using System.Collections.Generic;
using UnityEngine;

public class ExpertHeuristic : Decision
{

    public float noise = 0f;

    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        System.Random r = new System.Random();

        if (r.NextDouble() < noise)
        {
            return new float[] { r.Next(0, 3), r.Next(0, 3) };
        }

        if (vectorObs[21] == 1)
        {
            return new float[] { 0f, 2f };
        }

        if (vectorObs[20] == 1)
        {
            return new float[] { 0f, 1f };
        }

        if (vectorObs[19] == 1)
        {
            return new float[] { 1f, 0f };
        }

        if (vectorObs[18] == 1)
        {
            return new float[] { 2f, 0f };
        }
        
        return new float[] { 0f, r.Next(1, 3) };
    }
    
    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
