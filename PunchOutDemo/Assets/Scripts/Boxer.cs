using MLAgents;
using UnityEngine.Events;

public class Boxer : Agent
{

    /// <summary>
    /// The punch event
    /// </summary>
    public UnityEvent punch = new UnityEvent();

    /// <summary>
    /// The dodge event
    /// </summary>
    public UnityEvent dodge = new UnityEvent();

    /// <summary>
    /// The maximum health points
    /// </summary>
    public float maxHealth = 100;
    private float health;

    /// <summary>
    /// The multiplier to apply to incoming damage when blocking.
    /// </summary>
    public float blockMultiplier = 0.1f;

    /// <summary>
    /// The weak punch type
    /// </summary>
    public PunchType weakPunch = PunchType.STRAIGHT;

    /// <summary>
    /// The weak punch strength
    /// </summary>
    public float weakPunchStrength = 10;

    /// <summary>
    /// The strong punch type
    /// </summary>
    public PunchType strongPunch = PunchType.HOOK;

    /// <summary>
    /// The strong punch strength
    /// </summary>
    public float strongPunchStrength = 20;

    private DodgeState dodgeState = DodgeState.NONE;
    private Punch punchState = Punch.NULL_PUNCH;


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Awake()
    {
        health = maxHealth;
    }

    /// <summary>
    /// Collect the observations (internal punch)
    /// </summary>
    public override void CollectObservations()
    {
        // Current punch state
        AddVectorObs(punchState.GetHand() == Hand.RIGHT ? 1.0f : 0.0f);
        AddVectorObs(punchState.GetHand() == Hand.LEFT ? 1.0f : 0.0f);
        AddVectorObs(punchState.GetPunchType() == weakPunch ? 1.0f : 0.0f);
        AddVectorObs(punchState.GetPunchType() == strongPunch ? 1.0f : 0.0f);

        // Current dodge state
        AddVectorObs(dodgeState == DodgeState.LEFT ? 1.0f : 0.0f);
        AddVectorObs(dodgeState == DodgeState.RIGHT ? 1.0f : 0.0f);
        AddVectorObs(dodgeState == DodgeState.FRONT ? 1.0f : 0.0f);

        // Camera view of enemy?
        // Or maybe there is a reference to the opponent object in here
    }

    /// <summary>
    /// Take an action
    /// </summary>
    /// <param name="vectorAction">The action to take</param>
    /// <param name="textAction">The name of the action</param>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);
        HandleDodgeInput(vectorAction[0]);
        HandlePunchInput(vectorAction[1]);
    }

    /// <summary>
    /// Get the current dodge state of the boxer
    /// </summary>
    /// <returns>The dodge state</returns>
    public DodgeState GetDodgeState()
    {
        return dodgeState;
    }

    /// <summary>
    /// Get the current punch state of the boxer
    /// </summary>
    /// <returns>The punch state</returns>
    public Punch GetPunchState()
    {
        return punchState;
    }

    /// <summary>
    /// Get the current health level of the boxer
    /// </summary>
    /// <returns>The health level</returns>
    public float GetHealth()
    {
        return health;
    }

    /// <summary>
    /// Get the maximum health level of the boxer
    /// </summary>
    /// <returns>The maximum health level</returns>
    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Handles the damage taken from an opposing punch
    /// </summary>
    /// <param name="punch">The punch</param>
    /// <returns>The outcome of the punch</returns>
    public PunchOutcome onPunched(Punch punch)
    {

        // Dodged
        if (dodgeState == DodgeState.LEFT && punch.GetHand() == Hand.RIGHT)
        {
            return PunchOutcome.DODGED;
        } else if (dodgeState == DodgeState.RIGHT && punch.GetHand() == Hand.LEFT)
        {
            return PunchOutcome.DODGED;
        }

        // Blocked
        if (dodgeState != DodgeState.NONE)
        {
            TakeDamage(punch.GetStrength() * blockMultiplier);
            if (IsKO())
            {
                return PunchOutcome.KO;
            } else
            {
                return PunchOutcome.BLOCKED;
            }
        }

        // Hit
        TakeDamage(punch.GetStrength());
        if (IsKO())
        {
            return PunchOutcome.KO;
        }
        else
        {
            return PunchOutcome.HIT;
        }
    }

    /// <summary>
    /// Determines if the boxer is knocked out
    /// </summary>
    /// <returns>True if the boxer is KO, false otherwise</returns>
    public bool IsKO()
    {
        return health == 0;
    }

    /// <summary>
    /// Reset the dodge state of the boxer
    /// </summary>
    void ResetDodgeState()
    {
        if (dodgeState == DodgeState.NONE)
        {
            return;
        }
        dodgeState = DodgeState.NONE;
        dodge.Invoke();
    }

    /// <summary>
    /// Reset the punch state of the boxer
    /// </summary>
    void ResetPunchState()
    {
        if (punchState == Punch.NULL_PUNCH)
        {
            return;
        }
        punchState = Punch.NULL_PUNCH;
        punch.Invoke(); // Is this correct?
    }

    /// <summary>
    /// Take damage from a punch (decreases health)
    /// </summary>
    /// <param name="damage">The amount of damage to take</param>
    private void TakeDamage(float damage)
    {
        health -= damage;
    }

    /// <summary>
    /// Throw a punch
    /// </summary>
    /// <param name="punch">The punch</param>
    private void ThrowPunch(Punch punch) // TODO: Set timer
    {
        if (punchState != Punch.NULL_PUNCH || dodgeState != DodgeState.NONE)
        {
            return;
        }
        punchState = punch;
        this.punch.Invoke();
    }

    /// <summary>
    /// Dodge in the given direction
    /// </summary>
    /// <param name="dodgeDirection">The direction to dodge</param>
    private void Dodge(DodgeDirection dodgeDirection)
    {
        if (dodgeState != DodgeState.NONE || punchState != Punch.NULL_PUNCH)
        {
            return;
        }
        switch (dodgeDirection)
        {
            case DodgeDirection.LEFT:
                dodgeState = DodgeState.LEFT;
                break;
            case DodgeDirection.RIGHT:
                dodgeState = DodgeState.RIGHT;
                break;
            case DodgeDirection.FRONT:
                dodgeState = DodgeState.FRONT;
                break;
        }

        dodge.Invoke();
    }


    /// <summary>
    /// Handle the dodge input
    /// </summary>
    /// <param name="dodgeInput">The dodge input value</param>
    private void HandleDodgeInput(float dodgeInput)
    {
        if (dodgeInput == 1)
        {
            Dodge(DodgeDirection.LEFT);
        }
        else if (dodgeInput == 2)
        {
            Dodge(DodgeDirection.RIGHT);
        }
        else if (dodgeInput == 3)
        {
            Dodge(DodgeDirection.FRONT);
        }
        else if (dodgeInput == 4)
        {
            Dodge(DodgeDirection.FRONT);
        }
        else
        {
            ResetDodgeState();
        }
    }

    /// <summary>
    /// Handle the punch input
    /// </summary>
    /// <param name="punchInput">The punch input value</param>
    private void HandlePunchInput(float punchInput)
    {
        if (punchInput == 1)
        {
            ThrowPunch(new Punch(weakPunch, Hand.LEFT, weakPunchStrength));
        }
        else if (punchInput == 2)
        {
            ThrowPunch(new Punch(weakPunch, Hand.RIGHT, weakPunchStrength));
        }
        else if (punchInput == 3)
        {
            ThrowPunch(new Punch(strongPunch, Hand.LEFT, strongPunchStrength));
        }
        else if (punchInput == 4)
        {
            ThrowPunch(new Punch(strongPunch, Hand.RIGHT, strongPunchStrength));
        }
        else
        {
            ResetPunchState();
        }
    }
}
