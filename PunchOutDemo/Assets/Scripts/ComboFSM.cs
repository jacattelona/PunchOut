public class ComboFSM
{
    private FSM fsm;
    private int numStates;

    /// <summary>
    /// Default constructor
    /// </summary>
    public ComboFSM()
    {
        fsm = new FSM(0);

        // States
        // 0 = no combos
        // 1 = n lefts
        // 2 = n rights
        // 3 = left, right
        // 4 = right, left
        // 5 = left, right, left
        // 6 = right, left, right
        numStates = 7;

        // Actions
        // 0 = none
        // 1 = left
        // 2 = right

        // Single punches
        fsm.AddTransition(0, 1, 1);
        fsm.AddTransition(0, 2, 2);

        fsm.AddTransition(1, 2, 3);
        fsm.AddTransition(2, 1, 4);

        fsm.AddTransition(3, 1, 5);
        fsm.AddTransition(3, 2, 2);

        fsm.AddTransition(4, 1, 1);
        fsm.AddTransition(4, 2, 6);

        fsm.AddTransition(5, 1, 1);
        fsm.AddTransition(5, 2, 2);

        fsm.AddTransition(6, 1, 1);
        fsm.AddTransition(6, 2, 2);
    }

    /// <summary>
    /// Throw a punch and update the FSM
    /// </summary>
    /// <param name="punch">The punch</param>
    /// <returns>The new state of the FSM</returns>
    public int ThrowPunch(Punch punch)
    {
        switch (punch.GetHand())
        {
            case Hand.LEFT:
                fsm.TakeAction(1);
                break;
            case Hand.RIGHT:
                fsm.TakeAction(2);
                break;
            default:
                fsm.TakeAction(0);
                break;
        }
        return GetState();
    }

    /// <summary>
    /// Get the state of the FSM
    /// </summary>
    /// <returns>The current state</returns>
    public int GetState()
    {
        return fsm.GetCurrentState();
    }

    /// <summary>
    /// Get the total number of states
    /// </summary>
    /// <returns>The number of possible states</returns>
    public int GetTotalStates()
    {
        return numStates;
    }

    /// <summary>
    /// Reset the combo chain to state 0
    /// </summary>
    public void ResetComboChain()
    {
        fsm.OverrideCurrentState(0);
    }

}
