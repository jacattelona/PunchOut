using UnityEngine;

public class ComboTracker: MonoBehaviour
{

    public float comboTimeout;
    private Boxer boxer;

    private FSM fsm;
    private int numStates;
    private float lastPunchTime;

    void Start()
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

        lastPunchTime = 0;

        boxer = GetComponent<Boxer>();
        boxer.punchAction.action.AddListener(TrackPunch);
    }

    void Update()
    {
        if (Time.time - lastPunchTime >= comboTimeout)
        {
            ResetComboChain();
        }
    }

    private void TrackPunch(int side)
    {
        lastPunchTime = Time.time;
        fsm.TakeAction(side);
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
