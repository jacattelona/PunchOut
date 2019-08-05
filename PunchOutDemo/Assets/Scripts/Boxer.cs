using MLAgents;
using UnityEngine.Events;

public class Boxer : Agent
{

    public UnityEvent punch = new UnityEvent();
    public UnityEvent dodge = new UnityEvent();

    /**
     * The maximum health points 
     */
    public float maxHealth = 100;
    private float health;

    /**
     * The damage caused to an opponent when punching
     */
    public float punchDamage = 10;

    /**
     * The multiplier to apply to incoming damage when blocking. 
     */
    public float blockMultiplier = 0.1f;

    private DodgeState dodgeState = DodgeState.NONE;
    private Punch punchState = Punch.NULL_PUNCH;


    // Start is called before the first frame update
    void Awake()
    {
        health = maxHealth;
    }

    public override void CollectObservations()
    {
        // Internal senses
        AddVectorObs(punchState.GetHand() == Hand.RIGHT ? 1.0f : 0.0f);
        AddVectorObs(punchState.GetHand() == Hand.LEFT ? 1.0f : 0.0f);
        AddVectorObs(dodgeState == DodgeState.LEFT ? 1.0f : 0.0f);
        AddVectorObs(dodgeState == DodgeState.RIGHT ? 1.0f : 0.0f);
        AddVectorObs(dodgeState == DodgeState.FRONT ? 1.0f : 0.0f);

        // Camera view of enemy?
        // Or maybe there is a reference to the opponent object in here
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);

        if (vectorAction[0] == 1)
        {
            Dodge(DodgeDirection.LEFT);
        } else if (vectorAction[0] == 2)
        {
            Dodge(DodgeDirection.RIGHT);
        } else if (vectorAction[0] == 3)
        {
            Dodge(DodgeDirection.FRONT);
        } else if (vectorAction[0] == 4)
        {
            Dodge(DodgeDirection.FRONT);
        } else
        {
            ResetDodgeState();
        }

        if (vectorAction[1] == 1)
        { // TODO: Throw different punches here - make each have different damages
            ThrowPunch(new Punch(PunchType.STRAIGHT, Hand.LEFT, punchDamage));
        } else if (vectorAction[1] == 2)
        {
            ThrowPunch(new Punch(PunchType.STRAIGHT, Hand.RIGHT, punchDamage));
        } else
        {
            ResetPunchState();
        }
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
    /// Get the amount of damage given per punch
    /// </summary>
    /// <returns>The damage per punch</returns>
    public float GetPunchDamage()
    {
        return punchDamage;
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
            TakeDamage(punch.GetDamage() * blockMultiplier);
            if (IsKO())
            {
                return PunchOutcome.KO;
            } else
            {
                return PunchOutcome.BLOCKED;
            }
        }

        // Hit
        TakeDamage(punch.GetDamage());
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
}
