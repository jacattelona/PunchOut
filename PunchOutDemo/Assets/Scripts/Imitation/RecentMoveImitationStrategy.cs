using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecentMoveImitationStrategy : IImitationStrategy
{
    public float Execute(Boxer me, Boxer teacher, Reward reward)
    {
        ActionHistory teacherHistory = teacher.actionHistory;
        ActionHistory myHistory = me.actionHistory;

        ActionHistory.MLActionEvent teacherAction = teacherHistory.GetLastAction();
        ActionHistory.MLActionEvent studentAction = myHistory.GetLastAction();

        float dt = Mathf.Abs(teacherAction.time - studentAction.time);
        float maxTime = 0.75f;
        float rewardMultipler = (maxTime - Mathf.Clamp(dt, 0, maxTime)) / maxTime;

        if (teacherAction.action == studentAction.action)
        {
            return rewardMultipler * reward.imitationReward;
        }
        else
        {
            return rewardMultipler * reward.imitationPenalty;
        }
    }

}
