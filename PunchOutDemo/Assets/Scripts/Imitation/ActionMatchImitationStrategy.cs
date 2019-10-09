using UnityEngine;

public class ActionMatchImitationStrategy : IImitationStrategy
{
    public float Execute(Boxer me, Boxer teacher, Reward reward)
    {
        Hand myPunch = me.GetPunchState().GetHand();
        DodgeState myDodge = me.GetDodgeState();

        Hand teacherPunch = teacher.GetPunchState().GetHand();
        DodgeState teacherDodge = teacher.GetDodgeState();

        if (myPunch == teacherPunch && myDodge == teacherDodge)
        {
            return reward.imitationReward * Time.deltaTime;
        }
        else
        {
            return reward.imitationPenalty * Time.deltaTime;
        }
    }
}
