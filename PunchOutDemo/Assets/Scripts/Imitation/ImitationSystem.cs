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

    private IImitationStrategy strategy;

    void Start()
    {
        me = GetComponent<Boxer>();
        myRewards = me.rewards;
        lastMoveTime = 0;
        strategy = new RecentMoveImitationStrategy();
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

        float reward = strategy.Execute(me, teacher, myRewards);

        if (reward > 0) rewardEvent.Invoke();

        me.AddReward(reward);
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
