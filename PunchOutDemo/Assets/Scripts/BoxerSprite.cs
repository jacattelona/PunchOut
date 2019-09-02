using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (boxer.GetPunchState() != lastPunchState) // TODO: Use the unity events
        {

            // TODO: Draw different punches here
            if (boxer.GetPunchState().GetHand() == Hand.LEFT)
            {
                Transform t = this.transform.GetChild(0);
                t.localPosition = t.localPosition + PUNCH;
            }
            else if (boxer.GetPunchState().GetHand() == Hand.RIGHT)
            {
                Transform t = this.transform.GetChild(1);
                t.localPosition = t.localPosition + PUNCH;
            }
            else
            {
                Transform left = this.transform.GetChild(0);
                left.localPosition = LDEFAULT;

                Transform right = this.transform.GetChild(1);
                right.localPosition = RDEFAULT;
            }

            lastPunchState = boxer.GetPunchState();
        }


        if (boxer.GetDodgeState() != lastDodgeState) // TODO: Consider rotation
        {
            if (boxer.GetDodgeState() == DodgeState.FRONT)
            {
                Transform left = this.transform.GetChild(0);
                Transform right = this.transform.GetChild(1);

                //rotate both inward 45 degrees to show a blocking motion
                left.localEulerAngles = BLOCKANGLE * -1;
                right.localEulerAngles = BLOCKANGLE;
            } else if (boxer.GetDodgeState() == DodgeState.RIGHT)
            {
                this.transform.localPosition = DEFAULT + RDODGELOC;
            } else if (boxer.GetDodgeState() == DodgeState.LEFT)
            {
                this.transform.localPosition = DEFAULT + LDODGELOC;
            } else 
            {
                this.transform.localPosition = DEFAULT;
                Transform left = this.transform.GetChild(0);
                Transform right = this.transform.GetChild(1);

                left.localEulerAngles = new Vector3(0, 0, 0);
                right.localEulerAngles = new Vector3(0, 0, 0);
            }
            lastDodgeState = boxer.GetDodgeState();
        }
        
    }
}
