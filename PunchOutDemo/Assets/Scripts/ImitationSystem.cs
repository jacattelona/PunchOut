using System.Linq;
using UnityEngine;

public class ImitationSystem : MonoBehaviour
{

    private Boxer me;
    public Boxer teacher;

    private RewardComponent myRewards;

    public bool shouldImitate = true;
    public bool ignoreInaction = true;

    // Start is called before the first frame update
    void Start()
    {
        me = GetComponent<Boxer>();
        myRewards = GetComponent<RewardComponent>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!shouldImitate) { return; }

        if (ignoreInaction && teacher.lastActions.All(value => value == 0)) { return; }

        if (Enumerable.SequenceEqual(teacher.lastActions, me.lastActions))
        {
            me.AddReward(myRewards.imitationReward * Time.fixedDeltaTime);
        }
        else
        {
            me.AddReward(myRewards.imitationPenalty * Time.fixedDeltaTime);
        }
    }
}
