using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyHeuristic : Decision
{
    private float[] NOTHING = new float[] { 0f, 0f };
    private float[] LEFT_PUNCH = new float[] { 0f, 1f };
    private float[] RIGHT_PUNCH = new float[] { 0f, 2f };
    private float[] LEFT_DODGE= new float[] { 1f, 0f };
    private float[] RIGHT_DODGE = new float[] { 2f, 0f };

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

        if (vectorObs[0] == 1 && vectorObs[1] == 1) // Can punch / dodge
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
