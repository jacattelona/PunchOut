using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveRecorderSystem : MonoBehaviour
{

    private MoveHistory history;
    private Boxer boxer;

    // Start is called before the first frame update
    void Start()
    {
        history = GetComponent<MoveHistory>();
        boxer = GetComponent<Boxer>();
    }

    void FixedUpdate()
    {
        if (history == null) return;
        float[] move = new float[] {
            boxer.GetDodgeState() == DodgeState.LEFT ? 1f : 0f,
            boxer.GetDodgeState() == DodgeState.RIGHT ? 1f : 0f,
            boxer.GetPunchState().GetHand() == Hand.LEFT ? 1f : 0f,
            boxer.GetPunchState().GetHand() == Hand.RIGHT ? 1f : 0f
        };

        if (HasHistory())
        {
            MoveHistory.Move lastMove = GetLastMove();
            if (!Enumerable.SequenceEqual(lastMove.Actions, move))
            {
                history.moves.Add(new MoveHistory.Move { Actions = move, Time = Time.fixedTime });
            }
        } else
        {
            history.moves.Add(new MoveHistory.Move { Actions = move, Time = Time.fixedTime });
        }
    }

    public void Clear()
    {
        if (history == null) return;
        history.moves.Clear();
    }

    private bool HasHistory()
    {
        return history.moves.Count > 0;
    }

    private MoveHistory.Move GetLastMove()
    {
        return history.moves[history.moves.Count - 1];
    }
}
