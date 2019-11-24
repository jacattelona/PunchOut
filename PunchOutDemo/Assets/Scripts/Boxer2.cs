using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private float punchCooldownDuration;
    [SerializeField] private float dodgeCooldownDuration;


    [SerializeField] private float maxHP = 100;
    private float hp;

    [SerializeField] private float punchDamage = 10;

    private CooldownTimer punchCooldown;
    private CooldownTimer dodgeCooldown;

    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
        punchCooldown = new CooldownTimer(punchCooldownDuration);
        dodgeCooldown = new CooldownTimer(dodgeCooldownDuration);
        animator = GetComponent<Animator>();
        hp = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        punchCooldown.Update();
        dodgeCooldown.Update();
    }

    /// <summary>
    /// Collect the observations (internal punch)
    /// </summary>
    public override void CollectObservations()
    {
        AddVectorObs(state != State.LeftPunching && state != State.RightPunching && !punchCooldown.IsOnCooldown());
        AddVectorObs(state != State.LeftDodging && state != State.RightDodging && !dodgeCooldown.IsOnCooldown());

        bool[] move;

        if (true)
        {
            move = new bool[] { false, false, false, false };
        }

        AddVectorObs(Encoder.encodeInt(0, 0, 7));
        foreach (var m in move)
        {
            AddVectorObs(m);
        }
        AddVectorObs(state == State.LeftPunching);
        AddVectorObs(state == State.RightPunching);
        AddVectorObs(state == State.LeftDodging);
        AddVectorObs(state == State.RightDodging);
    }

    /// <summary>
    /// Take an action
    /// </summary>
    /// <param name="vectorAction">The action to take</param>
    /// <param name="textAction">The name of the action</param>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);

        switch (state)
        {
            case State.Idle:
                // Only perform confident moves
                var confidence = MLActionFactory.GetProbabilityFromVector(MLActionFactory.GetAction(vectorAction), vectorAction);
                if (!Mathf.Approximately(confidence, 0) && confidence < 0.1) return;

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
                if (IsIdleAnimation())
                {
                    state = State.Idle;
                    punchCooldown.Start();
                }
                break;
            case State.RightPunching:
                if (IsIdleAnimation())
                {
                    state = State.Idle;
                    punchCooldown.Start();
                }
                break;
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
                break;
            case MLAction.DODGE_RIGHT:
                animator.Play("DodgeRight");
                state = State.RightDodging;
                break;
            case MLAction.PUNCH_LEFT:
                animator.Play("PunchLeft");
                state = State.LeftPunching;
                break;
            case MLAction.PUNCH_RIGHT:
                animator.Play("PunchRight");
                state = State.RightPunching;
                break;
        }

    }
}
