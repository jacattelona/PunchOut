using MLAgents;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Boxer : Agent
{
    /// <summary>
    /// The boxing area
    /// </summary>
    public GameObject area;
    private BoxerArea myArea;

    public float punchCooldown = 0.1f;
    public float dodgeCooldown = 0.1f;
    public float punchDuration = 0.1f;
    public float dodgeDuration = 0.5f;
    public float punchEventDelay = 0.0f;
    public float dodgeEventDelay = 0.0f;

    private float lastPunchTime = -1f;

    public float gotPunchedPenalty = -0.01f;
    public float punchedReward = 0.01f;
    public float gotKOPenalty = 0f;
    public float koReward = 0f;
    public float existancePenalty = 0f;//-0.0003f;
    public float dodgedReward = 0.00f;
    public float dodgedPenalty = -0.00f;

    public int memorySize = 3;
    private MoveMemory moveMemory;
    private float lastOpponentPunchTime = -1f;

    private ComboFSM myComboTracker;
    public float comboTimeout = 1f;

    /// <summary>
    /// The name of the boxer
    /// </summary>
    public string name;

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

    private DodgeState dodgeState = DodgeState.NONE;
    private Punch punchState = Punch.NULL_PUNCH;

    public Action punchAction;
    public Action dodgeAction;


    /// <summary>
    /// Initialize the agent
    /// </summary>
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        myArea = area.GetComponent<BoxerArea>();
        health = maxHealth;
        moveMemory = new MoveMemory(memorySize, new float[] { 0f, 0f, 0f, 0f });
        myComboTracker = new ComboFSM();

        dodgeAction = new Action(dodgeDuration, dodgeCooldown, dodgeEventDelay);
        dodgeAction.action.AddListener(Dodge);
        dodgeAction.animationStart.AddListener(RegisterDodge);
        dodgeAction.animationEnd.AddListener(DeregisterDodge);

        punchAction = new Action(punchDuration, punchCooldown, punchEventDelay);
        punchAction.action.AddListener(ThrowPunch);
        punchAction.animationStart.AddListener(RegisterPunch);
        punchAction.animationEnd.AddListener(DeregisterPunch);
    }

    void Update()
    {
        punchAction.Update();
        dodgeAction.Update();
    }

    /// <summary>
    /// Collect the observations (internal punch)
    /// </summary>
    public override void CollectObservations()
    {
        AddVectorObs(Time.fixedTime - lastPunchTime >= punchDuration + punchCooldown); // Can punch

        float[] move;
        int opponentComboState = 0;

        if (name == "Player")
        {
            AddVectorObs(myArea.opponentBoxer.GetHealth() / myArea.opponentBoxer.GetMaxHealth());
            move = new float[] {
                myArea.opponentBoxer.GetPunchState().GetHand() == Hand.RIGHT ? 1f : 0f,
                myArea.opponentBoxer.GetPunchState().GetHand() == Hand.LEFT ? 1f : 0f,
                myArea.opponentBoxer.GetDodgeState() == DodgeState.LEFT ? 1f : 0f,
                myArea.opponentBoxer.GetDodgeState() == DodgeState.RIGHT ? 1f : 0f
            };

            opponentComboState = myArea.opponentBoxer.GetComboState();
        }
        else
        {
            AddVectorObs(myArea.playerBoxer.GetHealth() / myArea.opponentBoxer.GetMaxHealth());

            move = new float[] {
                myArea.playerBoxer.GetPunchState().GetHand() == Hand.RIGHT ? 1f : 0f,
                myArea.playerBoxer.GetPunchState().GetHand() == Hand.LEFT ? 1f : 0f,
                myArea.playerBoxer.GetDodgeState() == DodgeState.LEFT ? 1f : 0f,
                myArea.playerBoxer.GetDodgeState() == DodgeState.RIGHT ? 1f : 0f
            };

            opponentComboState = myArea.playerBoxer.GetComboState();
        }


        AddVectorObs(Encoder.encodeInt(myComboTracker.GetState(), 0, myComboTracker.GetTotalStates()));
        AddVectorObs(Encoder.encodeInt(opponentComboState, 0, myComboTracker.GetTotalStates()));
        AddVectorObs(move);
    }

    public int GetComboState()
    {
        return myComboTracker.GetState();
    }

    /// <summary>
    /// Take an action
    /// </summary>
    /// <param name="vectorAction">The action to take</param>
    /// <param name="textAction">The name of the action</param>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);
        if (IsKO())
        {
            return;
        }
        HandleDodgeInput(vectorAction[0]);
        HandlePunchInput(vectorAction[1]);
        AddReward(existancePenalty);
    }

    /// <summary>
    /// Reset the agent
    /// </summary>
    public override void AgentReset()
    {
        health = maxHealth;
        ResetDodgeState();
        ResetPunchState();
        moveMemory = new MoveMemory(memorySize, new float[] { 0f, 0f, 0f, 0f });
        lastOpponentPunchTime = -1;
        myComboTracker = new ComboFSM();
        Done();
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
        if (gameObject == myArea.player)
        {
            if (dodgeState == DodgeState.LEFT && punch.GetHand() == Hand.LEFT)
            {
                AddReward(dodgedReward);
                return PunchOutcome.DODGED;
            }
            else if (dodgeState == DodgeState.RIGHT && punch.GetHand() == Hand.RIGHT)
            {
                AddReward(dodgedReward);
                return PunchOutcome.DODGED;
            }
        }
        else
        {
            if (dodgeState == DodgeState.RIGHT && punch.GetHand() == Hand.LEFT)
            {
                AddReward(dodgedReward);
                return PunchOutcome.DODGED;
            }
            else if (dodgeState == DodgeState.LEFT && punch.GetHand() == Hand.RIGHT)
            {
                AddReward(dodgedReward);
                return PunchOutcome.DODGED;
            }
        }
        

        // Blocked
        //if (dodgeState != DodgeState.NONE)
        //{
        //    TakeDamage(punch.GetStrength() * blockMultiplier);
        //    if (IsKO())
        //    {
        //        AddReward(gotKOPenalty);
        //        return PunchOutcome.KO;
        //    } else
        //    {
        //        return PunchOutcome.BLOCKED;
        //    }
        //}

        // Hit
        TakeDamage(punch.GetStrength());
        if (IsKO())
        {
            AddReward(gotKOPenalty);
            return PunchOutcome.KO;
        }
        else
        {
            AddReward(gotPunchedPenalty * punch.GetStrength());
            return PunchOutcome.HIT;
        }
    }

    /// <summary>
    /// Determines if the boxer is knocked out
    /// </summary>
    /// <returns>True if the boxer is KO, false otherwise</returns>
    public bool IsKO()
    {
        return health <= 0;
    }

    /// <summary>
    /// Reset the dodge state of the boxer
    /// </summary>
    void ResetDodgeState()
    {
        dodgeAction.Reset();
        dodgeState = DodgeState.NONE;
    }

    void ResetPunchState()
    {
        punchAction.Reset();
        punchState = Punch.NULL_PUNCH;
        lastPunchTime = 0;
    }

    /// <summary>
    /// Take damage from a punch (decreases health)
    /// </summary>
    /// <param name="damage">The amount of damage to take</param>
    private void TakeDamage(float damage)
    {
        health -= damage;
    }

    private void RegisterDodge(int dodgeType)
    {
        if (dodgeType == 1)
        {
            dodgeState = DodgeState.LEFT;
        }
        else
        {
            dodgeState = DodgeState.RIGHT;
        }
    }

    private void DeregisterDodge(int dodgeType)
    {
        dodgeState = DodgeState.NONE;
    }

    private void RegisterPunch(int punchType)
    {
        Punch punch;
        if (punchType == 1)
        {
            punch = new Punch(weakPunch, Hand.LEFT, weakPunchStrength);
        }
        else
        {
            punch = new Punch(weakPunch, Hand.RIGHT, weakPunchStrength);
        }
        punchState = punch;
        lastPunchTime = Time.time;
    }

    private void DeregisterPunch(int punchType)
    {
        punchState = Punch.NULL_PUNCH;
    }

    /// <summary>
    /// Throw a punch
    /// </summary>
    /// <param name="punch">The punch</param>
    private void ThrowPunch(int punchType) // TODO: Set timer
    {
        this.punch.Invoke();
        myComboTracker.ThrowPunch(punchState);
    }

    /// <summary>
    /// Dodge in the given direction
    /// </summary>
    /// <param name="dodgeDirection">The direction to dodge</param>
    private void Dodge(int dodgeDirection)
    {
        this.dodge.Invoke();
    }


    /// <summary>
    /// Handle the dodge input
    /// </summary>
    /// <param name="dodgeInput">The dodge input value</param>
    private void HandleDodgeInput(float dodgeInput)
    {
        if (dodgeInput != 0 && !punchAction.IsRunning())
        {
            dodgeAction.Run((int)dodgeInput);
        }
    }

    /// <summary>
    /// Handle the punch input
    /// </summary>
    /// <param name="punchInput">The punch input value</param>
    private void HandlePunchInput(float punchInput)
    {

        if (punchInput != 0 && !dodgeAction.IsRunning())
        {
            punchAction.Run((int) punchInput);
        }
        
        if (Time.time - lastPunchTime >= comboTimeout)
        {
            myComboTracker.ResetComboChain();
        }
    }

    public void RewardOutcome(PunchOutcome outcome)
    {
        switch (outcome)
        {
            case PunchOutcome.BLOCKED:
                break;
            case PunchOutcome.DODGED:
                AddReward(dodgedPenalty);
                break;
            case PunchOutcome.HIT:
                AddReward(punchedReward * punchState.GetStrength());
                break;
            case PunchOutcome.KO:
                AddReward(koReward);
                break;
        }
    }
}
