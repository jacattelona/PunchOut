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
        float rewardMultipler = 1 - Mathf.Clamp01(dt);

        if (teacherAction.action == studentAction.action)
        {
            return rewardMultipler * reward.imitationReward * 0.1f;
        }
        else
        {
            return rewardMultipler * reward.imitationPenalty * 0.1f;
        }
    }

}
