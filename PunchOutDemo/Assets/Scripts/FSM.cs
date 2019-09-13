using System.Collections.Generic;

/// <summary>
/// A finite state machine
/// </summary>
public class FSM
{
    private int currentState;
    private List<Transition> transitions;

    /// <summary>
    /// Default constuctor
    /// </summary>
    /// <param name="initialState">The initial state of the FSM</param>
    public FSM(int initialState)
    {
        currentState = initialState;
        transitions = new List<Transition>();
    }

    /// <summary>
    /// Add a transition between states to the FSM
    /// </summary>
    /// <param name="startState">The starting state</param>
    /// <param name="action">The action which causes the transition</param>
    /// <param name="endState">The ending state</param>
    public void AddTransition(int startState, int action, int endState)
    {
        transitions.Add(new Transition(startState, endState, action));
    }

    /// <summary>
    /// Get the current state of the FSM
    /// </summary>
    /// <returns>The current state</returns>
    public int GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Take the action and update the current state
    /// </summary>
    /// <param name="action">The action to take</param>
    /// <returns>The state of the FSM after the action was taken</returns>
    public int TakeAction(int action)
    {
        foreach (Transition transition in transitions)
        {
            if (transition.startState == currentState && transition.action == action)
            {
                currentState = transition.endState;
                break;
            }
        }

        return currentState;
    }

    /// <summary>
    /// Override the current state of the FSM
    /// </summary>
    /// <param name="newCurrentState">The new current state of the FSM</param>
    public void OverrideCurrentState(int newCurrentState)
    {
        currentState = newCurrentState;
    }


    private class Transition
    {
        public int startState;
        public int endState;
        public int action;

        public Transition(int startState, int endState, int action)
        {
            this.startState = startState;
            this.endState = endState;
            this.action = action;
        }
    }
}
