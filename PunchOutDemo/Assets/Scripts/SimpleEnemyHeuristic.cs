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
    private int seqIdx;

    private MLAction[][] moveSequences;

    private float seqDuration = 30f;
    private float seqStartTime = 0f;
    private float nothingDuration = 1f;
    private float nothingStartTime = 0;
    private bool didAction = false;

    public static List<SimpleEnemyHeuristic> instances = new List<SimpleEnemyHeuristic>();


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

        moveSequences = new MLAction[][]
        {
            new MLAction[]{ MLAction.PUNCH_LEFT, MLAction.PUNCH_RIGHT },
            new MLAction[]{ MLAction.NOTHING },
            new MLAction[]{ MLAction.PUNCH_LEFT, MLAction.NOTHING, MLAction.PUNCH_RIGHT }
        };

        instances.Add(this);
    }

    private MLAction runMoves(List<float> vectorObs, MLAction[] moves)
    {
        MLInput input = new MLInput(vectorObs.ToArray());

        var currentMove = moves[moveIdx];

        if (currentMove == MLAction.NOTHING && Time.time - nothingStartTime >= nothingDuration)
        {
            // Doing nothing ended
            return runNextMove(moves, input);
        } else if (didAction && MLActionFactory.IsPunch(currentMove) && input.GetMyMove() == MLAction.NOTHING)
        {
            // Punch ended
            return runNextMove(moves, input);
        }
        else if (didAction && MLActionFactory.IsDodge(currentMove) && input.GetMyMove() == MLAction.NOTHING)
        {
            // Dodge ended
            return runNextMove(moves, input);
        }

        if (!didAction && MLActionFactory.IsPunch(currentMove))
        {
            didAction = input.IsPunchReady();
        }
        else if (!didAction && MLActionFactory.IsDodge(currentMove))
        {
            didAction = input.IsDodgeReady();
        }

        return moves[moveIdx];
    }

    private MLAction runNextMove(MLAction[] moves, MLInput input)
    {
        didAction = false;
        nothingStartTime = Time.time;
        moveIdx = (moveIdx + 1) % moves.Length;
        MLAction nextMove = moves[moveIdx];

        if (MLActionFactory.IsPunch(nextMove))
        {
            didAction = input.IsPunchReady();
        } else if (MLActionFactory.IsDodge(nextMove))
        {
            didAction = input.IsDodgeReady();
        }

        return nextMove;
    }

    public override float[] Decide(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {

        if (Mathf.Approximately(seqStartTime, 0))
        {
            Reset();
        }

        if (Time.time - seqStartTime > 30 && seqIdx == 0)
        {
            seqIdx = 1;
            moveIdx = 0;
            didAction = false;
            nothingStartTime = Time.time;
        } else if (Time.time - seqStartTime > 60 && seqIdx == 1)
        {
            seqIdx = 2;
            moveIdx = 0;
            didAction = false;
            nothingStartTime = Time.time;
        }

        MLAction nextMove = runMoves(vectorObs, moveSequences[seqIdx]);
        return MLActionFactory.GetVectorAction(nextMove);



        //if (done)
        //{
        //    moveIdx = 0;
        //    return NOTHING;
        //}


        //MLInput input = new MLInput(vectorObs.ToArray());

        //if (input.IsPunchReady() && input.IsDodgeReady()) // Can punch / dodge
        //{
        //    float[] move = moves[moveIdx];
        //    moveIdx = (moveIdx + 1) % moves.Length;
        //    return move;
        //}

        //return NOTHING;
    }

    public void Reset()
    {
        seqStartTime = Time.time;
        seqIdx = 0;
        moveIdx = 0;
        didAction = false;
        nothingStartTime = Time.time;
    }
    

    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
