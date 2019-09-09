﻿using MLAgents;
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

    private float lastPunchTime = -1f;
    private float lastDodgeTime = -1f;

    public float gotPunchedPenalty = -0.01f;
    public float punchedReward = 0.01f;
    public float gotKOPenalty = 0f;
    public float koReward = 0f;
    public float existancePenalty = 0f;//-0.0003f;
    public float dodgedReward = 0.00f;
    public float dodgedPenalty = -0.00f;

    private MoveMemory moveMemory;
    private float timeSinceOpponentMoveChange = 0f;

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
    /// Initialize the agent
    /// </summary>
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        myArea = area.GetComponent<BoxerArea>();
        health = maxHealth;
        moveMemory = new MoveMemory(5, new float[] { 0f, 0f, 0f, 0f });
    }

    /// <summary>
    /// Collect the observations (internal punch)
    /// </summary>
    public override void CollectObservations()
    {
        // Current punch state
        AddVectorObs(health / maxHealth);
        AddVectorObs(punchState.GetHand() == Hand.RIGHT);
        AddVectorObs(punchState.GetHand() == Hand.LEFT);
        //AddVectorObs(punchState.GetPunchType() == weakPunch ? 1.0f : 0.0f);
        //AddVectorObs(punchState.GetPunchType() == strongPunch ? 1.0f : 0.0f);

        // Current dodge state
        AddVectorObs(dodgeState == DodgeState.LEFT);
        AddVectorObs(dodgeState == DodgeState.RIGHT);
        //AddVectorObs(dodgeState == DodgeState.FRONT);

        AddVectorObs(Time.fixedTime - lastPunchTime >= punchDuration + punchCooldown); // Can punch
        AddVectorObs(Time.fixedTime - lastDodgeTime >= dodgeDuration + dodgeCooldown); // Can block 

        float[] move;

        if (gameObject == myArea.player)
        {
            AddVectorObs(myArea.opponentBoxer.GetHealth() / myArea.opponentBoxer.GetMaxHealth());
            move = new float[] {
                myArea.opponentBoxer.GetPunchState().GetHand() == Hand.RIGHT ? 1f : 0f,
                myArea.opponentBoxer.GetPunchState().GetHand() == Hand.LEFT ? 1f : 0f,
                myArea.opponentBoxer.GetDodgeState() == DodgeState.LEFT ? 1f : 0f,
                myArea.opponentBoxer.GetDodgeState() == DodgeState.RIGHT ? 1f : 0f
            };
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
        }

        timeSinceOpponentMoveChange++;

        if (!Enumerable.SequenceEqual(move, moveMemory.GetLastMove()))
        {
            moveMemory.Add(move);
            timeSinceOpponentMoveChange = 0f;
        }

        float[][] moves = moveMemory.Get();

        foreach(float[] m in moves)
        {
            AddVectorObs(m);
        }

        AddVectorObs(timeSinceOpponentMoveChange / 100f);
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
        lastPunchTime = -1;
        lastDodgeTime = -1;
        moveMemory = new MoveMemory(5, new float[] { 0f, 0f, 0f, 0f });
        timeSinceOpponentMoveChange = 0f;
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
        if (dodgeState == DodgeState.LEFT && punch.GetHand() == Hand.LEFT)
        {
            AddReward(dodgedReward);
            return PunchOutcome.DODGED;
        } else if (dodgeState == DodgeState.RIGHT && punch.GetHand() == Hand.RIGHT)
        {
            AddReward(dodgedReward);
            return PunchOutcome.DODGED;
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
        if (Time.fixedTime - lastDodgeTime < dodgeDuration) // Can't do anything while still dodging
        {
            return;
        }
        else
        {
            ResetDodgeState();
        }

        if (dodgeInput == 0)
        {
            return;
        }

        if (Time.fixedTime - lastDodgeTime > dodgeCooldown + dodgeDuration) // Can only dodge when past cooldown
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
            lastDodgeTime = Time.fixedTime;
        }
    }

    /// <summary>
    /// Handle the punch input
    /// </summary>
    /// <param name="punchInput">The punch input value</param>
    private void HandlePunchInput(float punchInput)
    {

        if (Time.fixedTime - lastPunchTime < punchDuration) // Can't do anything while still punching
        {
            return;
        }
        else
        {
            ResetPunchState();
        }

        if (punchInput == 0)
        {
            return;
        }

        if (Time.fixedTime - lastPunchTime > punchCooldown + punchDuration)
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
            lastPunchTime = Time.fixedTime;
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