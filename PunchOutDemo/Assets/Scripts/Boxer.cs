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
    private PunchState punchState = PunchState.NONE;


    // Start is called before the first frame update
    void Awake()
    {
        health = maxHealth;
    }

    public override void CollectObservations()
    {
        // Internal senses
        AddVectorObs(punchState == PunchState.RIGHT ? 1.0f : 0.0f);
        AddVectorObs(punchState == PunchState.LEFT ? 1.0f : 0.0f);
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
        {
            Punch(PunchSide.RIGHT);
        } else if (vectorAction[1] == 2)
        {
            Punch(PunchSide.LEFT);
        } else
        {
            ResetPunchState();
        }
    }

    public DodgeState GetDodgeState()
    {
        return dodgeState;
    }

    public PunchState GetPunchState()
    {
        return punchState;
    }

    public float GetPunchDamage()
    {
        return punchDamage;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    /**
     * Called when punched by the opponent 
     */
    public PunchOutcome onPunched(PunchSide punchSide, float incomingDamage)
    {

        // Dodged
        if (dodgeState == DodgeState.LEFT && punchSide == PunchSide.RIGHT)
        {
            return PunchOutcome.DODGED;
        } else if (dodgeState == DodgeState.RIGHT && punchSide == PunchSide.LEFT)
        {
            return PunchOutcome.DODGED;
        }

        // Blocked
        if (dodgeState != DodgeState.NONE)
        {
            TakeDamage(incomingDamage * blockMultiplier);
            if (IsKO())
            {
                return PunchOutcome.KO;
            } else
            {
                return PunchOutcome.BLOCKED;
            }
        }

        // Hit
        TakeDamage(incomingDamage);
        if (IsKO())
        {
            return PunchOutcome.KO;
        }
        else
        {
            return PunchOutcome.HIT;
        }
    }

    public bool IsKO()
    {
        return health == 0;
    }

    void ResetDodgeState()
    {
        dodgeState = DodgeState.NONE;
        dodge.Invoke();
    }

    void ResetPunchState()
    {
        punchState = PunchState.NONE;
        punch.Invoke();
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void Punch(PunchSide punchSide)
    {
        switch (punchSide)
        {
            case PunchSide.LEFT:
                // Update UI
                punchState = PunchState.LEFT;
                break;
            case PunchSide.RIGHT:
                // Update UI
                punchState = PunchState.RIGHT;
                break;
        }

        punch.Invoke();
    }

    private void Dodge(DodgeDirection dodeDirection)
    {
        switch (dodeDirection)
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
