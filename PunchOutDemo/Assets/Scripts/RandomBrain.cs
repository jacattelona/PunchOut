using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBrain : Decision
{
    private float[] NOTHING = MLActionFactory.GetVectorAction(MLAction.NOTHING);
    private float[] LEFT_PUNCH = MLActionFactory.GetVectorAction(MLAction.PUNCH_LEFT);
    private float[] RIGHT_PUNCH = MLActionFactory.GetVectorAction(MLAction.PUNCH_RIGHT);
    private float[] LEFT_DODGE = MLActionFactory.GetVectorAction(MLAction.DODGE_LEFT);
    private float[] RIGHT_DODGE = MLActionFactory.GetVectorAction(MLAction.DODGE_RIGHT);

    private float[][] moves;


    public RandomBrain()
    {
        moves = new float[][]
        {
            LEFT_PUNCH,
            RIGHT_PUNCH,
            LEFT_DODGE,
            RIGHT_DODGE,
            NOTHING
        };
    }


    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        int randIdx = Mathf.Clamp(Mathf.FloorToInt(Random.Range(0, moves.Length - 0.001f)), 0, moves.Length - 1);
        return moves[randIdx];
    }

    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
