using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A representation of the boxer sprite
/// </summary>
public class BoxerSprite : MonoBehaviour // TODO: Break this apart
{

    public Boxer boxer;

    //Const Values (you can't declare Vector3s const, so just pretend)
    protected Vector3 LDEFAULT = new Vector3(-1, 0, 0);             //Default position of the left glove
    protected Vector3 RDEFAULT = new Vector3(1, 0, 0);              //Default position of the right glove
    protected Vector3 PUNCH = new Vector3(0, 2, 0);                 //Distance a glove moves forward during punch

    Vector3 LDODGELOC = new Vector3(-2, 0, 0);                      //Number of units to move when dodging left
    Vector3 RDODGELOC = new Vector3(2, 0, 0);                       //Number of units to move when dodging right
    Vector3 BDODGELOC = new Vector3(0, -2, 0);                      //Number of units to move when dodging back
    Vector3 BLOCKANGLE = new Vector3(0, 0, 45);                     //Angle to move arms inward when blocking

    protected int punchTime = 0;                                    //number of frames left in punch animation
    private Punch lastPunchState;
    private DodgeState lastDodgeState;

    public Vector3 DEFAULT = new Vector3(0, 0, 0);                  //Default position of the boxer

    // Start is called before the first frame update
    void Start()
    {
        DEFAULT = this.transform.localPosition;
        lastPunchState = boxer.GetPunchState();
        lastDodgeState = boxer.GetDodgeState();
        boxer.dodgeAction.animationStart.AddListener(StartDodgeAnimation);
        boxer.dodgeAction.animationEnd.AddListener(StopDodgeAnimation);
        boxer.punchAction.animationStart.AddListener(StartPunchAnimation);
        boxer.punchAction.animationEnd.AddListener(StopPunchAnimation);
    }

    private void StartDodgeAnimation(int direction)
    {
        if (direction == 1)
        {
            this.transform.localPosition = DEFAULT + LDODGELOC;
        } else
        {
            this.transform.localPosition = DEFAULT + RDODGELOC;
        }
    }

    private void StopDodgeAnimation(int direction)
    {
        this.transform.localPosition = DEFAULT;
        Transform left = this.transform.GetChild(0);
        Transform right = this.transform.GetChild(1);

        left.localEulerAngles = new Vector3(0, 0, 0);
        right.localEulerAngles = new Vector3(0, 0, 0);
    }

    private void StartPunchAnimation(int side)
    {
        if (side == 1)
        {
            Transform t = this.transform.GetChild(0);
            t.localPosition = t.localPosition + PUNCH;
        } else
        {
            Transform t = this.transform.GetChild(1);
            t.localPosition = t.localPosition + PUNCH;
        }
    }

    private void StopPunchAnimation(int side)
    {
        Transform left = this.transform.GetChild(0);
        left.localPosition = LDEFAULT;

        Transform right = this.transform.GetChild(1);
        right.localPosition = RDEFAULT;
    }

    // Update is called once per frame
    void Update()
    {
        if (boxer.punchAction.IsOnCooldown() || boxer.dodgeAction.IsRunning() || boxer.punchAction.IsRunning())
        {
            // Lower opacity of punch icon
        } else
        {
            // Reset opacity of punch icon
        }

        if (boxer.dodgeAction.IsOnCooldown() || boxer.punchAction.IsRunning() || boxer.dodgeAction.IsRunning())
        {
            // Lower opacity of dodge icon
        }
        else
        {
            // Reset opacity of dodge icon
        }
    }
}
