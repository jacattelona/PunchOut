using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImitationSystem : MonoBehaviour
{

    private Boxer me;
    public Boxer teacher;

    public float matchingReward = 0.1f;
    public float differPenalty = -0.1f;

    public bool shouldImitate = true;

    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<Boxer>();
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
            me.AddReward(differPenalty);
            return;
        }


        if (teacherDodgeState.GetType() != myDodgeState.GetType())
        {
            me.AddReward(differPenalty);
            return;
        }

        me.AddReward(matchingReward);
    }
}
