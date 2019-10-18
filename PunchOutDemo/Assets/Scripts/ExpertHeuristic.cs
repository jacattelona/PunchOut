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

        MLInput input = new MLInput(vectorObs.ToArray());

        if (input.GetOpponentAction() == MLAction.DODGE_LEFT)
        {
            return MLActionFactory.GetVectorAction(MLAction.PUNCH_RIGHT);
        }

        if (input.GetOpponentAction() == MLAction.DODGE_RIGHT)
        {
            return MLActionFactory.GetVectorAction(MLAction.PUNCH_LEFT);
        }

        if (input.GetOpponentAction() == MLAction.PUNCH_LEFT)
        {
            return MLActionFactory.GetVectorAction(MLAction.DODGE_LEFT);
        }

        if (input.GetOpponentAction() == MLAction.PUNCH_RIGHT)
        {
            return MLActionFactory.GetVectorAction(MLAction.DODGE_RIGHT);
        }
        
        return new float[] { 0f, r.Next(1, 3) };
    }
    
    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
