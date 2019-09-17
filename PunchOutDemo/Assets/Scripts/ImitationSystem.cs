using UnityEngine;

public class ImitationSystem : MonoBehaviour
{

    private Boxer me;
    public Boxer teacher;

    private RewardComponent myRewards;

    public bool shouldImitate = true;

    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<Boxer>();
        myRewards = GetComponent<RewardComponent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!shouldImitate) { return;  }

        Punch teacherPunchState = teacher.GetPunchState();
        Punch myPunchState = me.GetPunchState();

        DodgeState teacherDodgeState = teacher.GetDodgeState();
        DodgeState myDodgeState = me.GetDodgeState();


        if (teacherPunchState.GetHand() != myPunchState.GetHand())
        {
            me.AddReward(myRewards.imitationPenalty);
            return;
        }


        if (teacherDodgeState.GetType() != myDodgeState.GetType())
        {
            me.AddReward(myRewards.imitationPenalty);
            return;
        }

        me.AddReward(myRewards.imitationReward);
    }
}
