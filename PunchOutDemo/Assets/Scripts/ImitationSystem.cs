using System.Linq;
using UnityEngine;

public class ImitationSystem : MonoBehaviour
{

    private Boxer me;
    public Boxer teacher;

    private Reward myRewards;

    public bool shouldImitate = true;
    public bool ignoreInaction = true;

    public KeyCode noOpKey = KeyCode.C;

    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<Boxer>();
        myRewards = me.rewards;
    }

    // Update is called once per frame
    void Update()
    {
        if (!shouldImitate) { return; }

        if (ignoreInaction && IsTeacherInactive() && !IsTeachingToPerformInaction()) { return; }

        if (ArePerformingSameAction())
        {
            me.AddReward(myRewards.imitationReward * Time.fixedDeltaTime);
        }
        else
        {
            me.AddReward(myRewards.imitationPenalty * Time.fixedDeltaTime);
        }
    }

    private bool ArePerformingSameAction()
    {
        // TODO: Maybe make this verify what actions they are currently doing instead
        return teacher.GetPunchState().GetHand() == me.GetPunchState().GetHand() && teacher.GetDodgeState() == me.GetDodgeState();
        //return Enumerable.SequenceEqual(teacher.lastActions, me.lastActions);
    }

    private bool IsTeachingToPerformInaction()
    {
        return Input.GetKey(noOpKey);
    }

    private bool IsTeacherInactive()
    {
        // Possibly only do this after X seconds of inactivity
        return teacher.lastActions.All(value => value == 0);
    }
}
