using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyHeuristic : Decision
{
    private int state = 0;
    private float lastTime = -1;
    private float maxTime = 0.6f;

    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        switch (state)
        {
            case 0:
                if (changeState(1))
                {
                    return new float[] { 0f, 0f };
                }
                break;
            case 1:
                if (changeState(2))
                {
                    return new float[] { 0f, 1f };
                }
                break;
            case 2:
                if (changeState(3))
                {
                    return new float[] { 0f, 0f };
                }
                break;
            case 3:
                if (changeState(4))
                {
                    return new float[] { 0f, 2f };
                }
                break;
            case 4:
                if (changeState(5))
                {
                    return new float[] { 0f, 0f };
                }
                break;
            case 5:
                if (changeState(0))
                {
                    return new float[] { 1f, 0f };
                }
                break;
        }
        return new float[] { 0f, 0f };
    }

    private bool changeState(int newState)
    {
        if (Time.fixedTime - lastTime >= maxTime)
        {
            state = newState;
            lastTime = Time.fixedTime;
            return true;
        }
        return false;
    }

    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
