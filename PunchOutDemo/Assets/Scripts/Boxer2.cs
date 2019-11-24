using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Boxer2 : Agent
{

    private enum State
    {
        Idle,
        LeftPunching,
        RightPunching,
        LeftDodging,
        RightDodging
    }

    private State state;

    private Boxer2 opponent;

    private BoxerAudio audio;

    [SerializeField] private float punchCooldownDuration;
    [SerializeField] private float dodgeCooldownDuration;


    [SerializeField] public float maxHP = 100;
    public float hp;

    [SerializeField] private float punchDamage = 10;

    [SerializeField] private float punchEventDelay;
    private float punchStartTime;
    private bool punchThrown = false;

    private CooldownTimer punchCooldown;
    private CooldownTimer dodgeCooldown;

    private Animator animator;

    private float lastLoss = float.PositiveInfinity;
    public UnityEvent hitEvent;
    public DirectionEvent punchEvent, dodgeEvent;
    ComboTracker comboTracker;

    public bool isFighting = false;

    public float[] lastActions;

    public float minConfidence = 0;

    public bool isTeacher = false;

    private int nothingBuffer = 0;
    private int nothingBufferSize = 1;


    [SerializeField] private bool inverted = false;

    [System.Serializable]
    public class DirectionEvent : UnityEvent<Direction>
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
        punchCooldown = new CooldownTimer(punchCooldownDuration);
        dodgeCooldown = new CooldownTimer(dodgeCooldownDuration);
        animator = GetComponent<Animator>();
        audio = GetComponent<BoxerAudio>();
        hp = maxHP;

        comboTracker = GetComponent<ComboTracker>();
        hitEvent = new UnityEvent();
        punchEvent = new DirectionEvent();
        dodgeEvent = new DirectionEvent();
    }

    // Update is called once per frame
    void Update()
    {
        punchCooldown.Update();
        dodgeCooldown.Update();
        minConfidence -= Time.deltaTime * 0.03f;
        minConfidence = Mathf.Clamp(minConfidence, 0.1f, 1);
    }

    /// <summary>
    /// Collect the observations (internal punch)
    /// </summary>
    public override void CollectObservations()
    {
        AddVectorObs(state != State.LeftPunching && state != State.RightPunching && !punchCooldown.IsOnCooldown());
        AddVectorObs(state != State.LeftDodging && state != State.RightDodging && !dodgeCooldown.IsOnCooldown());

        bool[] move;

        if (opponent != null)
        {
            move = new bool[] {
                opponent.GetCurrentAction() == MLAction.PUNCH_RIGHT,
                opponent.GetCurrentAction() == MLAction.PUNCH_LEFT,
                opponent.GetCurrentAction() == MLAction.DODGE_LEFT,
                opponent.GetCurrentAction() == MLAction.DODGE_RIGHT
            };

        }
        else
        {
            move = new bool[] { false, false, false, false };
        }

        AddVectorObs(Encoder.encodeInt(comboTracker.GetState(), 0, comboTracker.GetTotalStates()));
        foreach (var m in move)
        {
            AddVectorObs(m);
        }
        AddVectorObs(state == State.LeftPunching);
        AddVectorObs(state == State.RightPunching);
        AddVectorObs(state == State.LeftDodging);
        AddVectorObs(state == State.RightDodging);

        var training = isTeacher && isFighting && (MLActionFactory.GetAction(lastActions) != MLAction.NOTHING || nothingBuffer < nothingBufferSize);
        if (MLActionFactory.GetAction(lastActions) == MLAction.NOTHING) nothingBuffer++;
        SetTextObs(training + "," + false);
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

        switch (state)
        {
            case State.Idle:

                if (!isFighting) return;

                // Only perform confident moves
                var confidence = MLActionFactory.GetProbabilityFromVector(MLActionFactory.GetAction(vectorAction), vectorAction);
                if (!Mathf.Approximately(confidence, 0) && confidence < minConfidence) return;

                HandleInput(MLActionFactory.GetAction(vectorAction));
                animator.ResetTrigger("DodgeEnd");
                break;
            case State.LeftDodging:
                if (!Input.GetKey(KeyCode.D)) animator.SetTrigger("DodgeEnd");
                if (IsIdleAnimation())
                {
                    state = State.Idle;
                    dodgeCooldown.Start();
                }
                break;
            case State.RightDodging:
                if (!Input.GetKey(KeyCode.K))
                {
                    animator.SetTrigger("DodgeEnd");
                }
                if (IsIdleAnimation())
                {
                    state = State.Idle;
                    dodgeCooldown.Start();
                }
                break;
            case State.LeftPunching:
                TryHitPunch();
                if (IsIdleAnimation())
                {
                    state = State.Idle;
                    punchCooldown.Start();
                }
                break;
            case State.RightPunching:
                TryHitPunch();
                if (IsIdleAnimation())
                {
                    state = State.Idle;
                    punchCooldown.Start();
                }
                break;
        }
       
    }

    private void TryHitPunch()
    {
        if (Time.time - punchStartTime >= punchEventDelay && !punchThrown)
        {
            if (opponent)
            {
                opponent.OnPunched(state == State.LeftPunching ? Direction.LEFT : Direction.RIGHT, punchDamage);
                comboTracker.TrackPunch(state == State.LeftPunching ? Direction.LEFT : Direction.RIGHT);
            }
            punchThrown = true;
        }
    }

    private bool IsIdleAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Base");
    }

    private void HandleInput(MLAction requestedAction)
    {
        if (requestedAction == MLAction.NOTHING) return;

        // Ignore if on cooldown
        if (MLActionFactory.IsDodge(requestedAction) && dodgeCooldown.IsOnCooldown()) return;
        if (MLActionFactory.IsPunch(requestedAction) && punchCooldown.IsOnCooldown()) return;

        switch (requestedAction)
        {
            case MLAction.DODGE_LEFT:
                animator.Play("DodgeLeft");
                state = State.LeftDodging;
                dodgeEvent.Invoke(Direction.LEFT);
                audio?.PlayDodge();
                break;
            case MLAction.DODGE_RIGHT:
                animator.Play("DodgeRight");
                state = State.RightDodging;
                dodgeEvent.Invoke(Direction.RIGHT);
                audio?.PlayDodge();
                break;
            case MLAction.PUNCH_LEFT:
                animator.Play("PunchLeft");
                punchStartTime = Time.time;
                punchThrown = false;
                state = State.LeftPunching;
                punchEvent.Invoke(Direction.LEFT);
                break;
            case MLAction.PUNCH_RIGHT:
                animator.Play("PunchRight");
                punchStartTime = Time.time;
                punchThrown = false;
                state = State.RightPunching;
                punchEvent.Invoke(Direction.RIGHT);
                break;
        }

    }

    /// <summary>
    /// Take damage from a punch (decreases health)
    /// </summary>
    /// <param name="damage">The amount of damage to take</param>
    private void TakeDamage(float damage)
    {
        hp -= damage;
        hp = Mathf.Clamp(hp, 0, maxHP);
        hitEvent.Invoke();
        audio?.PlayHit();
    }

    /// <summary>
    /// Handles the damage taken from an opposing punch
    /// </summary>
    /// <param name="punch">The punch</param>
    /// <returns>The outcome of the punch</returns>
    public void OnPunched(Direction direction, float damage)
    {

        // Dodged
        if (!inverted) // If the player is flipped, this appears opposite
        {
            if (state == State.LeftDodging && direction == Direction.LEFT)
            {
                return;
            }
            else if (state == State.RightDodging && direction == Direction.RIGHT)
            {
                return;
            }
        }
        else
        {
            if (state == State.RightDodging && direction == Direction.LEFT)
            {
                return;
            }
            else if (state == State.LeftDodging && direction == Direction.RIGHT)
            {
                return;
            }
        }

        // Hit
        TakeDamage(damage);
    }

    /// <summary>
    /// Determines if the boxer is knocked out
    /// </summary>
    /// <returns>True if the boxer is KO, false otherwise</returns>
    public bool IsKO()
    {
        return hp <= 0;
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
    /// Tell the boxer to train on the previous rewards and reset the cumulative reward
    /// </summary>
    public void Train()
    {
        SetTextObs(false + "," + false);
        Done();
    }

    public MLAction GetCurrentAction()
    {
        switch (state)
        {
            case State.Idle: return MLAction.NOTHING;
            case State.LeftDodging: return MLAction.DODGE_LEFT;
            case State.RightDodging: return MLAction.DODGE_RIGHT;
            case State.LeftPunching: return MLAction.PUNCH_LEFT;
            case State.RightPunching: return MLAction.PUNCH_RIGHT;
        }

        return MLAction.NOTHING;
    }

    /// <summary>
    /// Reset the boxer's state
    /// </summary>
    public void ResetBoxer()
    {
        SetOpponent(null);
        Revive();
        state = State.Idle;
        comboTracker.ResetComboChain();
    }

    /// <summary>
    /// Revive the boxer (max out their health)
    /// </summary>
    public void Revive()
    {
        hp = maxHP;
    }

    /// <summary>
    /// Set the opponent that the boxer is fighting against
    /// </summary>
    /// <param name="boxer">The boxer</param>
    public void SetOpponent(Boxer2 boxer)
    {
        opponent = boxer;
    }


    private void PunchWarning(int side) { } // Placeholder

}
