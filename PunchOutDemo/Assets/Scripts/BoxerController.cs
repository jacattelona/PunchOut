using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerController : MonoBehaviour
{
    bool isDodging = false;
    /*
     * 0 = not dodging 
     * 1 = dodging left
     * 2 = dodging right
     * 3 = dodging back
     * 4 = blocking
     */
    int dodgeDirection = 0;

    bool isPunching = false;
    int punchTime = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        //InputManager receives input from keys on the keyboard (or any other source we want)
        //When those keys are pressed, it fires an event
        //BoxerController listens for those events, and executes a matching function when it detects them
        InputManager.Instance.LeftPunch.AddListener(LeftPunch);
        InputManager.Instance.RightPunch.AddListener(RightPunch);
        InputManager.Instance.LeftDodge.AddListener(LeftDodge);
        InputManager.Instance.RightDodge.AddListener(RightDodge);
        InputManager.Instance.BackDodge.AddListener(BackDodge);
        InputManager.Instance.Block.AddListener(Block);
        InputManager.Instance.Reset.AddListener(ResetDodges);
        //the name after InputManager.Instance  is the name of the Inputmanager Event
        //we call AddListener() on that event, giving the name of the function within BoxerController that we want to call

    }

    //called a guarenteed 50 times / second
    void FixedUpdate()
    {
        //if currently in the punch animation
        if (isPunching)
        {
            //wait 5 frames before resetting the fist back to default position
            punchTime--;
            if (punchTime == 0)
            {
                ResetPunches();
                isPunching = false;
            }
        }
    }

    void LeftPunch()
    {
        //if Not currently punching or dodging/blocking
        if (!isPunching && !isDodging)
        {
            //set boolean
            isPunching = true;
            //set a 5 frame timer
            punchTime = 5;
            //adjust the position of the left fist to show a punch
            Transform left = this.transform.GetChild(0);
            left.position = new Vector3(-1, 1, 0);
        } 
    }

    void RightPunch()
    {
        if (!isPunching && !isDodging)
        {
            isPunching = true;
            punchTime = 5;
            Transform right = this.transform.GetChild(1);
            right.position = new Vector3(1, 1, 0);
        }
        

    }



    void LeftDodge()
    {
        if (!isDodging && !isPunching)
        {
            isDodging = true;
            dodgeDirection = 1;
            this.transform.position = new Vector3(-2, 0, 0);
        }

    }
    void RightDodge()
    {
        if (!isDodging && !isPunching)
        {
            isDodging = true;
            dodgeDirection = 2;
            this.transform.position = new Vector3(2, 0, 0);
        }

    }

    void BackDodge()
    {
        if (!isDodging && !isPunching)
        {
            isDodging = true;
            dodgeDirection = 3;
            this.transform.position = new Vector3(0, -2, 0);
        }

    }


    void Block()
    {
        if (!isDodging && !isPunching)
        {
            isDodging = true;
            dodgeDirection = 4;

            //get transforms of left arm and right arm
            Transform left = this.transform.GetChild(0);
            Transform right = this.transform.GetChild(1);

            //rotate both inward 45 degrees to show a blocking motion
            left.eulerAngles = new Vector3(0, 0, -45);
            right.eulerAngles = new Vector3(0, 0, 45);
        }

    }



    private void ResetDodges()
    {
        //reset dodge boolean
        isDodging = false;
        //set position to default
        this.transform.position = Vector3.zero;

        //if we were blocking, reset arm rotation to 0
        if (dodgeDirection == 4)
        {
            Transform left = this.transform.GetChild(0);
            Transform right = this.transform.GetChild(1);

            left.eulerAngles = new Vector3(0, 0, 0);
            right.eulerAngles = new Vector3(0, 0, 0);
        }
        dodgeDirection = 0;
    }


    private void ResetPunches()
    {
        //bring both arms to a default position
        isPunching = false;

        Transform left = this.transform.GetChild(0);
        left.position = new Vector3(-1, 0, 0);

        Transform right = this.transform.GetChild(1);
        right.position = new Vector3(1, 0, 0);
    }

}
