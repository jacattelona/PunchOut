using UnityEngine;

public class MLOutputMatchImitationStrategy : IImitationStrategy
{
    public float Execute(Boxer me, Boxer teacher, Reward reward)
    {
        MLAction myAction = MLActionFactory.GetAction(me.lastActions);
        MLAction teacherAction = MLActionFactory.GetAction(teacher.lastActions);

        if (myAction == teacherAction)
        {
            return reward.imitationReward * Time.deltaTime;
        }
        else
        {
            return reward.imitationPenalty * Time.deltaTime;
        }
    }
}
