using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgent : BoxerAgent
{

    //Const Values (you can't declare Vector3s const, so just pretend)
    Vector3 DRAWBACK = new Vector3(0, -2, 0);           //Distance a glove draws back while charging
    const int CHARGETIME = 50;                          //Number of frames spent drawing back
    const int ATTACKTIME = 20;                          //Number of frames spent with fist out


    Transform trans;                                    //Transform of the fist currently punching
    Vector3 a;                                          //Vector A used for fist interpolation during drawback (position to lerp from)
    Vector3 b;                                          //Vector B used for fist interpolation during drawback (position to lerp to)


    //Awake
    //Description: sets initial values as object is created
    //Params: none
    //Returns: none
    void Awake()
    {
        //Set trans and a to left fist values as a default, just to be safe
        trans = transform.GetChild(0);
        a = LDEFAULT;
        DEFAULT = new Vector3(0, 3, 0);
    }


    //FixedUpdate
    //Description: called 50 times / second, handles punch animation
    //Parameters: none
    //Returns: none
    void FixedUpdate()
    {
        //If currently punching
        if (isPunching)
        {
            //Decrement the punch frame counter
            punchTime--;

            //If we are in the drawing back phase of the punch
            if (punchTime > ATTACKTIME)
            {
                //b is the default position plus total drawback distance
                b = a + DRAWBACK;

                //t is a float from 0-1 indicating how far between a and b
                float t = (punchTime-ATTACKTIME) / (float)CHARGETIME;

                //Set localPosition to lerped value
                trans.localPosition = Vector3.Lerp(b, a, t);
            }

            //If we have finished punching
            else if (punchTime == 0)
            {
                //set fist back to default position, set isPunching to false
                trans.localPosition = a;
                isPunching = false;
            }

            //If we are in the attack phase of the punch
            else
            {
                //Set localPosition to default + punch distance
                trans.localPosition = a + PUNCH;
            }
        }


        
    }


    //LeftPunch
    //Descrition: Calls EnemyAgent's version of Punch
    //Parameters: None
    //Returns: None
    public override void LeftPunch()
    {
        Punch(true);
    }

    //RightPunch
    //Description: Calls EnemyAgent's Version of Punch
    //Parameters: None
    //Returns: None
    public override void RightPunch()
    {
        Punch(false);
    }

    //Punch
    //Description: Starts the punching animation for the specified glove
    //Parameters:
    //  left (bool): indicates left or right glove
    //Returns: none
    private void Punch(bool left)
    {
        //If not currently dodging or punching
        if (!isDodging && !isPunching)
        {
            //Set isPunching, and set trans and a to proper glove values
            isPunching = true;
            if (left)
            {
                trans = transform.GetChild(0);
                a = LDEFAULT;
            }
            else
            {
                trans = transform.GetChild(1);
                a = RDEFAULT;
            }

            //Start the punch animation counter
            punchTime = CHARGETIME + ATTACKTIME;
        }
    }
}
