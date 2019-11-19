using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Action
{
    /// <summary>
    /// The cooldown time of the action in seconds
    /// </summary>
    public float cooldownTime;

    /// <summary>
    /// The total duration of the action in seconds (including animation)
    /// </summary>
    private float duration;

    /// <summary>
    /// The time since the start of the action to trigger the action's event in seconds
    /// </summary>
    private float actionTriggerTime;

    /// <summary>
    /// The event that triggers when the action occurs (may occur after animation start)
    /// </summary>
    public UnityEvent<Direction> action;

    /// <summary>
    /// The event that triggers when the animation is started
    /// </summary>
    public UnityEvent<Direction> animationStart;

    /// <summary>
    /// The event that triggers when the animation is stopped
    /// </summary>
    public UnityEvent<Direction> animationEnd;


    /// <summary>
    /// The last start time of the action in seconds
    /// </summary>
    private float lastStartTime;

    /// <summary>
    /// The last finish time of the action in seconds
    /// </summary>
    private float lastFinishTime;

    private FSM state;

    private const int ACTION_START = 0;
    private const int ACTION_INTERRUPT = 1;
    private const int ACTION_TRIGGER_EVENT = 2;
    private const int ACTION_COOLDOWN_OVER = 3;
    private const int ACTION_ANIMATION_OVER = 4;

    private const int STATE_READY = 0;
    private const int STATE_ON_COOLDOWN = 1;
    private const int STATE_ANIMATION_STARTED = 2;
    private const int STATE_EVENT_FIRED = 3;

    private Direction data;

    [System.Serializable]
    private class DirectionEvent : UnityEvent<Direction>
    {
    }

    public Action(float cooldownTime)
    {
        state = new FSM(STATE_READY);
        state.AddTransition(STATE_READY, ACTION_START, STATE_ANIMATION_STARTED);
        state.AddTransition(STATE_ANIMATION_STARTED, ACTION_TRIGGER_EVENT, STATE_EVENT_FIRED);
        state.AddTransition(STATE_ANIMATION_STARTED, ACTION_INTERRUPT, STATE_ON_COOLDOWN);
        state.AddTransition(STATE_EVENT_FIRED, ACTION_INTERRUPT, STATE_ON_COOLDOWN);
        //state.AddTransition(STATE_EVENT_FIRED, ACTION_ANIMATION_OVER, STATE_ON_COOLDOWN);
        state.AddTransition(STATE_ON_COOLDOWN, ACTION_COOLDOWN_OVER, STATE_READY);

        this.duration = 100f;
        this.cooldownTime = cooldownTime;
        this.actionTriggerTime = 0f;
        action = new DirectionEvent();
        animationStart = new DirectionEvent();
        animationEnd = new DirectionEvent();

    }

    public Action(float duration, float cooldownTime, float actionTriggerTime = 0.0f)
    {
        state = new FSM(STATE_READY);

        state.AddTransition(STATE_READY, ACTION_START, STATE_ANIMATION_STARTED);
        state.AddTransition(STATE_ANIMATION_STARTED, ACTION_TRIGGER_EVENT, STATE_EVENT_FIRED);
        state.AddTransition(STATE_ANIMATION_STARTED, ACTION_INTERRUPT, STATE_ON_COOLDOWN);
        state.AddTransition(STATE_EVENT_FIRED, ACTION_INTERRUPT, STATE_ON_COOLDOWN);
        state.AddTransition(STATE_EVENT_FIRED, ACTION_ANIMATION_OVER, STATE_ON_COOLDOWN);
        state.AddTransition(STATE_ON_COOLDOWN, ACTION_COOLDOWN_OVER, STATE_READY);

        this.duration = duration;
        this.cooldownTime = cooldownTime;
        this.actionTriggerTime = actionTriggerTime;
        action = new DirectionEvent();
        animationStart = new DirectionEvent();
        animationEnd = new DirectionEvent();
    }

    /// <summary>
    /// Determines if the action is on cooldown
    /// </summary>
    /// <returns>True if the action is on cooldown, false otherwise</returns>
    public bool IsOnCooldown()
    {
        return state.GetCurrentState() == STATE_ON_COOLDOWN;
    }

    /// <summary>
    /// Determines if the action is currently running
    /// </summary>
    /// <returns>True if the action is running, false otherwise</returns>
    public bool IsRunning()
    {
        return state.GetCurrentState() == STATE_ANIMATION_STARTED || state.GetCurrentState() == STATE_EVENT_FIRED;
    }

    /// <summary>
    /// Run the action
    /// </summary>
    public void Run(Direction data)
    {
        if (state.GetCurrentState() != STATE_READY)
        {
            return;
        }

        this.data = data;

        // Switch states
        lastStartTime = Time.time;
        animationStart.Invoke(data);
        state.TakeAction(ACTION_START);
    }

    public void Update()
    {
        switch (state.GetCurrentState())
        {
            case STATE_ANIMATION_STARTED:
                if (Time.time - lastStartTime >= actionTriggerTime)
                {
                    action.Invoke(data);
                    state.TakeAction(ACTION_TRIGGER_EVENT);
                }
                break;
            case STATE_EVENT_FIRED:
                if (Time.time - lastStartTime >= duration)
                {
                    animationEnd.Invoke(data);
                    lastFinishTime = Time.time;
                    state.TakeAction(ACTION_ANIMATION_OVER);
                }
                break;
            case STATE_ON_COOLDOWN:
                if (Time.time - lastFinishTime >= cooldownTime)
                {
                    state.TakeAction(ACTION_COOLDOWN_OVER);
                }
                break;
        }
    }

    public void Reset()
    {
        Interrupt();
        state.OverrideCurrentState(STATE_READY);
    }


    public void Interrupt()
    {
        switch (state.GetCurrentState())
        {
            case STATE_ANIMATION_STARTED:
            case STATE_EVENT_FIRED:
                animationEnd.Invoke(data);
                lastFinishTime = Time.time;
                state.TakeAction(ACTION_INTERRUPT);
                break;
        }
    }


    public Direction GetData()
    {
        return data;
    }


}
