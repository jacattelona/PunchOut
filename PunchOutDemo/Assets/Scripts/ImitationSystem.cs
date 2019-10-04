using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ImitationSystem : MonoBehaviour
{

    private Boxer me;
    public Boxer teacher;

    private Reward myRewards;

    public bool shouldImitate = true;
    public bool ignoreInaction = true;

    public KeyCode noOpKey = KeyCode.C;

    public float inactivityTimeout = 2;

    private float lastMoveTime;

    public UnityEvent rewardEvent = new UnityEvent();

    void Start()
    {
        me = GetComponent<Boxer>();
        myRewards = me.rewards;
        lastMoveTime = 0;
    }

    void Update()
    {
        // Only runs if the boxer should be imitating something
        if (!shouldImitate || !me.isFighting || !teacher.isFighting) { return; }

        // If inaction should be ignored and the teacher is inactive
        if (ignoreInaction && IsTeacherInactive() && !IsTeachingToPerformInaction()) {
            // Teacher is inactive, so don't reward boxer
            return;
        }

        if (ArePerformingSameAction())
        {
            // Reward the boxer for acting the same as the teacher
            me.AddReward(myRewards.imitationReward * Time.fixedDeltaTime);
            rewardEvent.Invoke();
        }
        else
        { 
            // Penalize the boxer for acting differently from the teacher
            me.AddReward(myRewards.imitationPenalty * Time.fixedDeltaTime);
        }
    }

    private bool ArePerformingSameAction()
    {
        return teacher.GetPunchState().GetHand() == me.GetPunchState().GetHand() && teacher.GetDodgeState() == me.GetDodgeState();
    }

    private bool IsTeachingToPerformInaction()
    {
        return Input.GetKey(noOpKey);
    }

    private bool IsTeacherInactive()
    {
        if (!IsTeacherDoingNothing())
        {
            lastMoveTime = Time.time;
            return false;
        } else
        {
            return Time.time - lastMoveTime >= inactivityTimeout;
        }
    }

    private bool IsTeacherDoingNothing()
    {
        return teacher.GetPunchState().GetHand() == Hand.NONE && teacher.GetDodgeState() == DodgeState.NONE;
    }
}
