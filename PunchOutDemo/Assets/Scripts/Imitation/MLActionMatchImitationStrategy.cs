using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLActionMatchImitationStrategy : IImitationStrategy
{
    public float Execute(Boxer me, Boxer teacher, Reward reward)
    {
        MLAction myAction = MLActionFactory.GetAction(me.lastActions);
        MLAction myCurrentAction = me.currentAction;
        MLAction teacherAction = teacher.currentAction;

        if (myCurrentAction == teacherAction && myAction == MLAction.NOTHING)
        {
            // Doing the move, but nothing else
            return reward.imitationReward * Time.deltaTime;
        }

        if (myAction == teacherAction)
        {
            // Trying to do the same move as the teacher
            return reward.imitationReward * Time.deltaTime;
        }
        else
        {
            // Not doing the same move as the teacher
            return reward.imitationPenalty * Time.deltaTime;
        }
    }
}
