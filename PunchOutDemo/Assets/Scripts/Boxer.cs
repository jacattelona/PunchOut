﻿using MLAgents;
using UnityEngine;
using UnityEngine.Events;

public class Boxer : Agent
{
    private Animator anim;

    private Boxer opponent;

    private BoxerStats stats;

    private bool wasDodging = false;

    public bool allowPunchWhileDodging = false;

    public bool broadcastPunch = false;

    public float[] lastActions;

    public float punchCooldown = 0.1f;
    public float dodgeCooldown = 0.1f;
    public float punchDuration = 0.1f;
    public float dodgeDuration = 0.5f;
    public float punchEventDelay = 0.0f;
    public float dodgeEventDelay = 0.0f;

    public bool isFighting = false;
    public bool inverted = false;

    public float minConfidence = 0;

    public bool isTeacher = false;

    public Reward rewards;


    // COMPONENTS
    Health health;
    ComboTracker comboTracker;

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

    public ActionHistory actionHistory;

    public UnityEvent hitEvent;

    public MLAction currentAction;

    private float lastBufferResetTime;
    private float maxBufferResetTime = 1.5f;


    /// <summary>
    /// Initialize the agent
    /// </summary>
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        stats = new BoxerStats();
        health = GetComponent<Health>();
        comboTracker = GetComponent<ComboTracker>();
        lastBufferResetTime = Time.time;

        //dodgeAction = new Action(dodgeDuration, dodgeCooldown, dodgeEventDelay);
        dodgeAction = new Action(dodgeCooldown);
        dodgeAction.animationStart.AddListener(RegisterDodge);
        dodgeAction.animationEnd.AddListener(DeregisterDodge);

        punchAction = new Action(punchDuration, punchCooldown, punchEventDelay);
        punchAction.animationStart.AddListener(RegisterPunch);
        punchAction.animationEnd.AddListener(DeregisterPunch);

        hitEvent = new UnityEvent();

        actionHistory = new ActionHistory();

        currentAction = MLAction.NOTHING;

        //SetActionMask(0, new int[] { 1, 2, 3, 4 });
    }

    void FixedUpdate()
    {
        punchAction.Update();
        dodgeAction.Update();

        if (isTeacher)
            HandleEndDodge();
    }

    /// <summary>
    /// Collect the observations (internal punch)
    /// </summary>
    public override void CollectObservations()
    {
        AddVectorObs(!punchAction.IsOnCooldown() && !punchAction.IsRunning());
        AddVectorObs(!dodgeAction.IsOnCooldown() && !dodgeAction.IsRunning());

        float[] move;
        int opponentComboState = 0;

        if (opponent != null)
        {
            move = new float[] {
                opponent.GetPunchState().GetHand() == Hand.RIGHT ? 1f : 0f,
                opponent.GetPunchState().GetHand() == Hand.LEFT ? 1f : 0f,
                opponent.GetDodgeState() == DodgeState.LEFT ? 1f : 0f,
                opponent.GetDodgeState() == DodgeState.RIGHT ? 1f : 0f
            };

            opponentComboState = opponent.comboTracker.GetState();
        } else
        {
            move = new float[] { 0, 0, 0, 0 };
            opponentComboState = 0;
        }

        AddVectorObs(Encoder.encodeInt(comboTracker.GetState(), 0, comboTracker.GetTotalStates()));
        //AddVectorObs(Encoder.encodeInt(opponentComboState, 0, comboTracker.GetTotalStates()));
        AddVectorObs(move);
        AddVectorObs(currentAction == MLAction.PUNCH_LEFT);
        AddVectorObs(currentAction == MLAction.PUNCH_RIGHT);
        AddVectorObs(currentAction == MLAction.DODGE_LEFT);
        AddVectorObs(currentAction == MLAction.DODGE_RIGHT);
        if (Time.time - lastBufferResetTime > maxBufferResetTime)
        {
            SetTextObs((isTeacher && isFighting) + "," + true);
            lastBufferResetTime = Time.time;

        } else
        {
            SetTextObs((isTeacher && isFighting) + "," + false);
        }
    }

    /// <summary>
    /// Take an action
    /// </summary>
    /// <param name="vectorAction">The action to take</param>
    /// <param name="textAction">The name of the action</param>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);
        lastActions = vectorAction;

        var confidence = MLActionFactory.GetProbabilityFromVector(MLActionFactory.GetAction(vectorAction), vectorAction);

        if (!isFighting) return;

        // Only perform confident moves
        if (!Mathf.Approximately(confidence, 0) && confidence < minConfidence) return;

        if (vectorAction[0] == 0)
        {
            HandleDodgeInput(0);
        }
        // TODO: Neaten this up
        else if (vectorAction[0] <= 2)
        {
            HandlePunchInput(vectorAction[0]);
        } else
        {
            HandleDodgeInput(vectorAction[0] - 2);
        }
        if (rewards != null) AddReward(rewards.existancePenalty);
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
    /// Handles the damage taken from an opposing punch
    /// </summary>
    /// <param name="punch">The punch</param>
    /// <returns>The outcome of the punch</returns>
    public PunchOutcome onPunched(Punch punch)
    {

        stats.AddOpponentPunch();

        // Dodged
        if (!inverted) // If the player is flipped, this appears opposite
        {
            if (dodgeState == DodgeState.LEFT && punch.GetHand() == Hand.LEFT)
            {
                if (rewards != null) AddReward(rewards.dodgeReward);
                stats.AddSuccessfulDodge();
                return PunchOutcome.DODGED;
            }
            else if (dodgeState == DodgeState.RIGHT && punch.GetHand() == Hand.RIGHT)
            {
                if (rewards != null) AddReward(rewards.dodgeReward);
                stats.AddSuccessfulDodge();
                return PunchOutcome.DODGED;
            }
        }
        else
        {
            if (dodgeState == DodgeState.RIGHT && punch.GetHand() == Hand.LEFT)
            {
                if (rewards != null) AddReward(rewards.dodgeReward);
                stats.AddSuccessfulDodge();
                return PunchOutcome.DODGED;
            }
            else if (dodgeState == DodgeState.LEFT && punch.GetHand() == Hand.RIGHT)
            {
                if (rewards != null) AddReward(rewards.dodgeReward);
                stats.AddSuccessfulDodge();
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
            if (rewards != null) AddReward(rewards.knockOutPenalty);
            return PunchOutcome.KO;
        }
        else
        {
            if (rewards != null) AddReward(rewards.punchPenalty);
            return PunchOutcome.HIT;
        }
    }

    /// <summary>
    /// Determines if the boxer is knocked out
    /// </summary>
    /// <returns>True if the boxer is KO, false otherwise</returns>
    public bool IsKO()
    {
        return health.health <= 0;
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
    }

    /// <summary>
    /// Take damage from a punch (decreases health)
    /// </summary>
    /// <param name="damage">The amount of damage to take</param>
    private void TakeDamage(float damage)
    {
        health.health -= (int) damage;
        hitEvent.Invoke();

    }

    private void RegisterDodge(int dodgeType)
    {
        stats.AddDodge();
        actionHistory.Record(dodgeType == 1 ? MLAction.DODGE_LEFT : MLAction.DODGE_RIGHT);
        if (dodgeType == 1)
        {
            dodgeState = DodgeState.LEFT;
            currentAction = MLAction.DODGE_LEFT;
        }
        else
        {
            dodgeState = DodgeState.RIGHT;
            currentAction = MLAction.DODGE_RIGHT;
        }
    }

    private void DeregisterDodge(int dodgeType)
    {
        dodgeState = DodgeState.NONE;
        currentAction = MLAction.NOTHING;
    }

    private void RegisterPunch(int punchType)
    {
        stats.AddPunch();
        actionHistory.Record(punchType == 1 ? MLAction.PUNCH_LEFT : MLAction.PUNCH_RIGHT);
        Punch punch;
        if (punchType == 1)
        {
            punch = new Punch(weakPunch, Hand.LEFT, weakPunchStrength);
            currentAction = MLAction.PUNCH_LEFT;
        }
        else
        {
            punch = new Punch(weakPunch, Hand.RIGHT, weakPunchStrength);
            currentAction = MLAction.PUNCH_RIGHT;
        }
        punchState = punch;
    }

    private void DeregisterPunch(int punchType)
    {
        punchState = Punch.NULL_PUNCH;
        currentAction = MLAction.NOTHING;
    }


    /// <summary>
    /// Handle the dodge input
    /// </summary>
    /// <param name="dodgeInput">The dodge input value</param>
    private void HandleDodgeInput(float dodgeInput)
    {
        int input = (int)dodgeInput;
        if (input == 0 && wasDodging)
        {
            //if (isTeacher) print("End Dodge");
            dodgeAction.Interrupt();
            wasDodging = false;
        }


        if (dodgeInput != 0 && (allowPunchWhileDodging || !punchAction.IsRunning()))
        {
            wasDodging = true;
            //if (isTeacher) print("Started dodge");
            dodgeAction.Run(input);
        }
    }

    private void HandleEndDodge()
    {
        if (!Input.GetKey(KeyCode.D) && dodgeState == DodgeState.LEFT)
        {
            print("Ending left dodge");
            dodgeAction.Interrupt();
        }
        else if (!Input.GetKey(KeyCode.K) && dodgeState == DodgeState.RIGHT)
        {
            print("Ending right dodge");
            dodgeAction.Interrupt();
        }
    }

    /// <summary>
    /// Handle the punch input
    /// </summary>
    /// <param name="punchInput">The punch input value</param>
    private void HandlePunchInput(float punchInput)
    {

        if (punchInput != 0 && (allowPunchWhileDodging || !dodgeAction.IsRunning()))
        {
            punchAction.Run((int) punchInput);

        }
    }

    public void RewardOutcome(PunchOutcome outcome)
    {
        switch (outcome)
        {
            case PunchOutcome.BLOCKED:
                break;
            case PunchOutcome.DODGED:
                if (rewards != null) AddReward(rewards.dodgePenalty);
                break;
            case PunchOutcome.HIT:
                if (rewards != null) AddReward(rewards.punchReward);
                stats.AddHit();
                break;
            case PunchOutcome.KO:
                if (rewards != null) AddReward(rewards.knockOutReward);
                stats.AddHit();
                break;
        }
    }

    /// <summary>
    /// Tell the boxer to train on the previous rewards and reset the cumulative reward
    /// </summary>
    public void Train()
    {
        Done();
    }

    /// <summary>
    /// Get the current score of the boxer (how they are performaning in the fight)
    /// </summary>
    /// <returns>The performance score of the boxer</returns>
    public float GetPerformanceScore()
    {
        return GetCumulativeReward();
    }

    public float GetOverallScore()
    {
        return GetPerformanceScore(); // TODO: make this a weighted combination of hit percentage, reward, dodge percentage, etc
    }

    /// <summary>
    /// Reset the boxer's state
    /// </summary>
    public void ResetBoxer()
    {
        SetOpponent(null);
        Revive();
        ResetDodgeState();
        ResetPunchState();
        comboTracker.ResetComboChain();
    }

    /// <summary>
    /// Revive the boxer (max out their health)
    /// </summary>
    public void Revive()
    {
        health.health = health.maxHealth;
    }

    /// <summary>
    /// Set the opponent that the boxer is fighting against
    /// </summary>
    /// <param name="boxer">The boxer</param>
    public void SetOpponent(Boxer boxer)
    {
        opponent = boxer;
    }

    /// <summary>
    /// Warning event that an enemy punch is about to be thrown
    /// </summary>
    /// <param name="side">side punch will be thrown from: 1 = left, 2 = right</param>
    public void PunchWarning(int side)
    {
        if (side == 1)
        {
            //left punch
        }
        else if (side == 2)
        {
            //right punch
        }
    }
}
