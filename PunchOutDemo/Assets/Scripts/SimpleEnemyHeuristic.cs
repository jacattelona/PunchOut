using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyHeuristic : Decision
{
    private float[] NOTHING = MLActionFactory.GetVectorAction(MLAction.NOTHING);
    private float[] LEFT_PUNCH = MLActionFactory.GetVectorAction(MLAction.PUNCH_LEFT);
    private float[] RIGHT_PUNCH = MLActionFactory.GetVectorAction(MLAction.PUNCH_RIGHT);
    private float[] LEFT_DODGE= MLActionFactory.GetVectorAction(MLAction.DODGE_LEFT);
    private float[] RIGHT_DODGE = MLActionFactory.GetVectorAction(MLAction.DODGE_RIGHT);

    private float[][] moves;

    private int moveIdx;

    public SimpleEnemyHeuristic()
    {
        moves = new float[][]
        {
            LEFT_PUNCH,
            RIGHT_PUNCH,
            LEFT_PUNCH,
            RIGHT_DODGE,
            RIGHT_PUNCH
        };

        moveIdx = 0;
    }

    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        if (done)
        {
            moveIdx = 0;
            return NOTHING;
        }

        MLInput input = new MLInput(vectorObs.ToArray());

        if (input.IsPunchReady() && input.IsDodgeReady()) // Can punch / dodge
        {
            float[] move = moves[moveIdx];
            moveIdx = (moveIdx + 1) % moves.Length;
            return move;
        }

        return NOTHING;
    }
    

    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
