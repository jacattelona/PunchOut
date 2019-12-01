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

    private MLAction[] moves;

    private float seqDuration = 30f;
    private float seqStartTime = 0f;
    private float nothingDuration = 2.5f;
    private float nothingStartTime = 0;
    private bool didAction = false;

    private int moveIdx;

    public AIEnemyHeuristic()
    {
        moves = new MLAction[]
        {
            MLAction.PUNCH_LEFT,
            MLAction.PUNCH_LEFT,
            MLAction.PUNCH_RIGHT,
            MLAction.NOTHING,
            MLAction.PUNCH_RIGHT,
            MLAction.PUNCH_RIGHT,
            MLAction.PUNCH_LEFT,
            MLAction.NOTHING,
            MLAction.PUNCH_RIGHT,
            MLAction.PUNCH_RIGHT,
            MLAction.PUNCH_RIGHT,
            MLAction.NOTHING,
            MLAction.PUNCH_LEFT,
            MLAction.PUNCH_LEFT,
            MLAction.PUNCH_LEFT

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

        if (input.GetMyMove() == MLAction.NOTHING && input.IsDodgeReady()) // Can dodge
        {
            if(input.GetOpponentAction() == MLAction.PUNCH_LEFT )
            {
                int rand = Random.Range(0, 100);
                if (rand <= 30)
                {
                    return LEFT_DODGE;
                }
                else if(rand <= 50 )
                {
                    return RIGHT_DODGE;
                }
                else
                {
                   // Run the normal moves
                }
            }
            if (input.GetOpponentAction() == MLAction.PUNCH_RIGHT)
            {
                int rand = Random.Range(0, 100);
                if (rand <= 30)
                {
                    return RIGHT_DODGE;
                }
                else if(rand <= 50)
                {
                    return LEFT_DODGE;
                }
                else
                {
                    // Run the normal moves
                }
            }
        }

        return MLActionFactory.GetVectorAction(runMoves(vectorObs, moves));
    }

    private MLAction runMoves(List<float> vectorObs, MLAction[] moves)
    {
        MLInput input = new MLInput(vectorObs.ToArray());

        var currentMove = moves[moveIdx];

        if (currentMove == MLAction.NOTHING && Time.time - nothingStartTime >= nothingDuration)
        {
            // Doing nothing ended
            return runNextMove(moves, input);
        }
        else if (didAction && MLActionFactory.IsPunch(currentMove) && input.GetMyMove() == MLAction.NOTHING)
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
        }
        else if (MLActionFactory.IsDodge(nextMove))
        {
            didAction = input.IsDodgeReady();
        }

        return nextMove;
    }


    public override List<float> MakeMemory(List<float> vectorObs, List<Texture2D> visualObs, float reward, bool done, List<float> memory)
    {
        // DO nothing
        return memory;
    }
}
