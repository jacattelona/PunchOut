using MLAgents;
using UnityEngine;
using UnityEngine.Events;

public class Boxer : Agent
{
    private Animator anim;

    private Boxer opponent;

    private BoxerStats stats;

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

    private int nothingBuffer = 0;
    private int nothingBufferSize = 1;

    public Reward rewards;


    // COMPONENTS
    Health health;
    ComboTracker comboTracker;

    /// <summary>
    /// The multiplier to apply to incoming damage when blocking.
    /// </summary>
    public float blockMultiplier = 0.1f;


    /// <summary>
    /// The weak punch strength
    /// </summary>
    public float weakPunchStrength = 10;

    public Action punchAction;
    public Action dodgeAction;

    public UnityEvent hitEvent;

    public MLAction currentAction;

    private int bufferSize;
    private int maxBufferSize = 2000;

    private int punchCount, dodgeCount;

    private float nothingDuration = 0;

    private float lastLoss = float.PositiveInfinity;


    /// <summary>
    /// Initialize the agent
    /// </summary>
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        stats = new BoxerStats();
        health = GetComponent<Health>();
        comboTracker = GetComponent<ComboTracker>();
        bufferSize = 0;

        dodgeAction = new Action(dodgeDuration, dodgeCooldown, dodgeEventDelay);
        dodgeAction.animationStart.AddListener(RegisterDodge);
        dodgeAction.animationEnd.AddListener(DeregisterDodge);

        punchAction = new Action(punchDuration, punchCooldown, punchEventDelay);
        punchAction.animationStart.AddListener(RegisterPunch);
        punchAction.animationEnd.AddListener(DeregisterPunch);

        hitEvent = new UnityEvent();

        currentAction = MLAction.NOTHING;

        //SetActionMask(0, new int[] { 1, 2, 3, 4 });
    }

    private void Update()
    {
        punchAction.Update();
        dodgeAction.Update();
        minConfidence -= Time.deltaTime * 0.03f;
        minConfidence = Mathf.Clamp(minConfidence, 0.2f, 1);
    }

    /// <summary>
    /// Collect the observations (internal punch)
    /// </summary>
    public override void CollectObservations()
    {
        AddVectorObs(!punchAction.IsOnCooldown() && !punchAction.IsRunning());
        AddVectorObs(!dodgeAction.IsOnCooldown() && !dodgeAction.IsRunning());

        bool[] move;
        int opponentComboState = 0;

        if (opponent != null)
        {
            move = new bool[] {
                opponent.currentAction == MLAction.PUNCH_RIGHT,
                opponent.currentAction == MLAction.PUNCH_LEFT,
                opponent.currentAction == MLAction.DODGE_LEFT,
                opponent.currentAction == MLAction.DODGE_RIGHT
            };

            opponentComboState = opponent.comboTracker.GetState();
        } else
        {
            move = new bool[] { false, false, false, false };
            opponentComboState = 0;
        }

        AddVectorObs(Encoder.encodeInt(comboTracker.GetState(), 0, comboTracker.GetTotalStates()));
        foreach(var m in move)
        {
            AddVectorObs(m);
        }
        AddVectorObs(currentAction == MLAction.PUNCH_LEFT);
        AddVectorObs(currentAction == MLAction.PUNCH_RIGHT);
        AddVectorObs(currentAction == MLAction.DODGE_LEFT);
        AddVectorObs(currentAction == MLAction.DODGE_RIGHT);

        if (bufferSize >= maxBufferSize)
        {
            SetTextObs((isTeacher && isFighting && MLActionFactory.GetAction(lastActions) != MLAction.NOTHING) + "," + true);
            bufferSize = 0;

        } else
        {
            if (MLActionFactory.IsPunch(MLActionFactory.GetAction(lastActions)))
            {
                punchCount++;
            } else if (MLActionFactory.IsDodge(MLActionFactory.GetAction(lastActions)))
            {
                dodgeCount++;
            }

            if (isTeacher)
            {
                Debug.Log(punchCount + ", " + dodgeCount);
            }

            var training = isTeacher && isFighting && (MLActionFactory.GetAction(lastActions) != MLAction.NOTHING || nothingBuffer < nothingBufferSize);
            if (training) bufferSize++;
            if (MLActionFactory.GetAction(lastActions) == MLAction.NOTHING) nothingBuffer++;
            SetTextObs(training + "," + false);
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

        lastLoss = MLActionFactory.GetLoss(vectorAction);

        if (!isFighting) return;

        // Only perform confident moves
        var confidence = MLActionFactory.GetProbabilityFromVector(MLActionFactory.GetAction(vectorAction), vectorAction);
        if (!Mathf.Approximately(confidence, 0) && confidence < minConfidence) return;

        // Try to perform action
        TryToTakeAction(MLActionFactory.GetAction(vectorAction));
    }


    private void TryToTakeAction(MLAction action)
    {
        if (IsPerformingMove()) return;

        if (MLActionFactory.IsPunch(action))
        {
            if (punchAction.IsOnCooldown()) return;
            punchAction.Run(action == MLAction.PUNCH_LEFT ? Direction.LEFT : Direction.RIGHT);
        }

        if (MLActionFactory.IsDodge(action))
        {
            if (dodgeAction.IsOnCooldown()) return;
            dodgeAction.Run(action == MLAction.DODGE_LEFT ? Direction.LEFT : Direction.RIGHT);
        }

    }

    private bool IsPerformingMove()
    {
        return currentAction != MLAction.NOTHING;
    }

    /// <summary>
    /// Handles the damage taken from an opposing punch
    /// </summary>
    /// <param name="punch">The punch</param>
    /// <returns>The outcome of the punch</returns>
    public PunchOutcome onPunched(MLAction action, float damage)
    {

        stats.AddOpponentPunch();

        // Dodged
        if (!inverted) // If the player is flipped, this appears opposite
        {
            if (currentAction == MLAction.DODGE_LEFT && action == MLAction.PUNCH_LEFT)
            {
                stats.AddSuccessfulDodge();
                return PunchOutcome.DODGED;
            }
            else if (currentAction == MLAction.DODGE_RIGHT && action == MLAction.PUNCH_RIGHT)
            {
                stats.AddSuccessfulDodge();
                return PunchOutcome.DODGED;
            }
        }
        else
        {
            if (currentAction == MLAction.DODGE_RIGHT && action == MLAction.PUNCH_LEFT)
            {
                stats.AddSuccessfulDodge();
                return PunchOutcome.DODGED;
            }
            else if (currentAction == MLAction.DODGE_LEFT && action == MLAction.PUNCH_RIGHT)
            {
                stats.AddSuccessfulDodge();
                return PunchOutcome.DODGED;
            }
        }
        
        // Hit
        TakeDamage(damage);
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
        return health.health <= 0;
    }

    /// <summary>
    /// Reset the dodge state of the boxer
    /// </summary>
    void ResetDodgeState()
    {
        dodgeAction.Reset();
    }

    void ResetPunchState()
    {
        punchAction.Reset();
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

    private void RegisterDodge(Direction direction)
    {
        stats.AddDodge();
        currentAction = direction == Direction.LEFT ? MLAction.DODGE_LEFT : MLAction.DODGE_RIGHT;
    }

    private void DeregisterDodge(Direction direction)
    {
        currentAction = MLAction.NOTHING;
    }

    private void RegisterPunch(Direction direction)
    {
        stats.AddPunch();
        currentAction = direction == Direction.LEFT ? MLAction.PUNCH_LEFT : MLAction.PUNCH_RIGHT;
    }

    private void DeregisterPunch(Direction direction)
    {
        currentAction = MLAction.NOTHING;
    }

    public void RewardOutcome(PunchOutcome outcome)
    {
        switch (outcome)
        {
            case PunchOutcome.BLOCKED:
                break;
            case PunchOutcome.DODGED:
                break;
            case PunchOutcome.HIT:
                stats.AddHit();
                break;
            case PunchOutcome.KO:
                stats.AddHit();
                break;
        }
    }

    /// <summary>
    /// Tell the boxer to train on the previous rewards and reset the cumulative reward
    /// </summary>
    public void Train()
    {
        SetTextObs(false + "," + false);
        Done();
    }

    /// <summary>
    /// Get the current cloning loss of the boxer
    /// </summary>
    /// <returns>The cloning loss (cross-entropy)</returns>
    public float GetLoss()
    {
        return lastLoss;
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

    public MLAction GetCurrentAction()
    {
        return currentAction;
    }

    public float GetStrength()
    {
        return weakPunchStrength;
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
