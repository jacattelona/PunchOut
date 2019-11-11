using MLAgents;
using System.Collections.Generic;
using UnityEngine;

public class ModifiedPlayerBrain : Decision
{

    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        MLInput input = new MLInput(vectorObs.ToArray());


        MLAction currentMove = input.GetMyMove();

        if (currentMove != MLAction.NOTHING)
        {
            return MLActionFactory.GetVectorAction(currentMove);
        }

        if (Input.GetKey(KeyCode.F))
        {
            return MLActionFactory.GetVectorAction(MLAction.PUNCH_LEFT);
        } else if (Input.GetKey(KeyCode.J))
        {
            return MLActionFactory.GetVectorAction(MLAction.PUNCH_RIGHT);
        } else if (Input.GetKey(KeyCode.D))
        {
            return MLActionFactory.GetVectorAction(MLAction.DODGE_LEFT);
        } else if (Input.GetKey(KeyCode.K))
        {
            return MLActionFactory.GetVectorAction(MLAction.DODGE_RIGHT);
        }

        return MLActionFactory.GetVectorAction(MLAction.NOTHING);
    }

    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
