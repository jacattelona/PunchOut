using MLAgents;
using System.Collections.Generic;
using UnityEngine;

public class ExpertHeuristic : Decision
{

    public float noise = 0f;
    private MLAction lastAction;

    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        return OnlyDoAndDodge(vectorObs, MLAction.PUNCH_LEFT);
    }

    public float[] OnlyDo(List<float> vectorObs, MLAction action)
    {
        return MLActionFactory.GetVectorAction(action);
    }

    public float[] OnlyDoAndDodge(List<float> vectorObs, MLAction action)
    {
        MLInput input = new MLInput(vectorObs.ToArray());

        if (lastAction != input.GetOpponentAction() && input.GetOpponentAction() == MLAction.PUNCH_LEFT)
        {
            lastAction = input.GetOpponentAction();
            return MLActionFactory.GetVectorAction(MLAction.DODGE_RIGHT);
        }

        if (lastAction != input.GetOpponentAction() && input.GetOpponentAction() == MLAction.PUNCH_RIGHT)
        {
            lastAction = input.GetOpponentAction();
            return MLActionFactory.GetVectorAction(MLAction.DODGE_LEFT);
        }

        lastAction = input.GetOpponentAction();

        return MLActionFactory.GetVectorAction(action);
    }
   

    public float[] ExpertDecide(List<float> vectorObs)
    {
        System.Random r = new System.Random();

        if (r.NextDouble() < noise)
        {
            return new float[] { r.Next(0, 5) };
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

        return new float[] { 0f };
    }
    
    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
