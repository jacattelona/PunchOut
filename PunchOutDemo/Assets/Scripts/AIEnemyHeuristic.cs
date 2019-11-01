using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemyHeuristic : Decision
{
    private float[] NOTHING = MLActionFactory.GetVectorAction(MLAction.NOTHING);
    private float[] LEFT_PUNCH = MLActionFactory.GetVectorAction(MLAction.PUNCH_LEFT);
    private float[] RIGHT_PUNCH = MLActionFactory.GetVectorAction(MLAction.PUNCH_RIGHT);
    private float[] LEFT_DODGE= MLActionFactory.GetVectorAction(MLAction.DODGE_LEFT);
    private float[] RIGHT_DODGE = MLActionFactory.GetVectorAction(MLAction.DODGE_RIGHT);

    private float[][] moves;

    private int moveIdx;

    public AIEnemyHeuristic()
    {
        moves = new float[][]
        {
            LEFT_PUNCH,
            LEFT_PUNCH,
            RIGHT_PUNCH,
            NOTHING,
            RIGHT_PUNCH,
            RIGHT_PUNCH,
            LEFT_PUNCH,
            NOTHING

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
            if(input.GetOpponentAction.equals( LEFT_PUNCH) )
            {
                if(Random.Range(0,100) <= 75)
                {
                    Console.Log("Left P Dodge");
                    return LEFT_DODGE;
                }
                else
                {
                    Console.Log("Left P Wrong Dodge");
                    return RIGHT_DODGE;
                }
            }
            if (input.GetOpponentAction.equals( RIGHT_PUNCH))
            {
                if (Random.Range(0, 100) <= 75)
                {
                    Console.Log("Right P Dodge");
                    return RIGHT_DODGE;
                }
                else
                {
                    Console.Log("Right P Wrong Dodge");
                    return LEFT_DODGE;
                }
            }
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
