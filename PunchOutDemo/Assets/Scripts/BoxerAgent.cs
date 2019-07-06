using MLAgents;
using TMPro;
using UnityEngine;

public class BoxerAgent : Agent
{ 
    public bool keepingScore = false;
    public TextMeshPro score;

    private int lastAction = -1;
    private int timeBetweenPunches = 0;

    //Const Values (you can't declare Vector3s const, so just pretend)
    protected Vector3 LDEFAULT = new Vector3(-1, 0, 0);             //Default position of the left glove
    protected Vector3 RDEFAULT = new Vector3(1, 0, 0);              //Default position of the right glove
    protected Vector3 PUNCH = new Vector3(0, 2, 0);                 //Distance a glove moves forward during punch

    Vector3 LDODGELOC = new Vector3(-2, 0, 0);                      //Number of units to move when dodging left
    Vector3 RDODGELOC = new Vector3(2, 0, 0);                       //Number of units to move when dodging right
    Vector3 BDODGELOC = new Vector3(0, -2, 0);                      //Number of units to move when dodging back
    Vector3 BLOCKANGLE = new Vector3(0, 0, 45);                     //Angle to move arms inward when blocking

    //dodge directions
    const int CENTER = 0;                                           //dodgeDirection for no dodge
    const int LDODGE = 1;                                           //dodgeDirection for left dodge
    const int RDODGE = 2;                                           //dodgeDirection for right dodge
    const int BDODGE = 3;                                           //dodgeDirection for back dodge
    const int BLOCK = 4;                                            //dodgeDirection for block

    const int PUNCHMAX = 5;                                         //max number of frames in punch animation



    protected bool isDodging = false;                               //is boxer currently dodging
    protected int dodgeDirection = 0;                               //direction boxer is dodging
    protected int revMult = 1;                                      //multiplier if directions should be reversed
    protected bool isPunching = false;                              //is boxer currently punching
    protected int punchTime = 0;                                    //number of frames left in punch animation


    public bool reverse = false;                                    //determines if directions should be reversed
    public Vector3 DEFAULT = new Vector3(0, 0, 0);                  //Default position of the boxer


    //Awake
    //Description: sets default values, called when object is created
    //Parameters: None
    //Returns: None
    void Awake()
    {
        //Set reverse multiplier if necessary
        if (reverse)
            revMult = -1;

        DEFAULT = this.transform.localPosition;
    }

    public override void CollectObservations()
    {
        float lastMove = lastAction == 1 ? 0f : 1f;

        AddVectorObs(lastAction);
    }


    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);

        if (vectorAction[0] == 1)
        {
            LeftDodge();
        }

        else if (vectorAction[0] == 2)
        {
            RightDodge();
        }

        else if (vectorAction[0] == 3)
        {
            BackDodge();
        }

        else if (vectorAction[0] == 4)
        {
            Block();
        }

        else 
        {
            ResetDodges();
        }

        if (vectorAction[1] == 1)
        {
            LeftPunch();
            if (lastAction == 2)
            {
                AddReward(1f);
                timeBetweenPunches = 0;
            }
            lastAction = 1;
        }

        else if (vectorAction[1] == 2)
        {
            RightPunch();
            if (lastAction == 1)
            {
                AddReward(1f);
                timeBetweenPunches = 0;
            }
            lastAction = 2;
        }

        if (timeBetweenPunches > 100)
        {
            Done();
        }


        // If hit,
        if (keepingScore)
        {          
            score.text = Mathf.Round(GetCumulativeReward()) + "";
        }

        timeBetweenPunches++;
    }

    //FixedUpdate
    //Description: called 50 times / second, handles punching animation
    //Parameters: None
    //Returns: None
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


    //LeftPunch
    //Description: Calls Punch
    //Parameters: None
    //Returns: None
    public virtual void LeftPunch()
    {
        Punch(true);
    }

    //LeftPunch
    //Description: Calls Punch
    //Parameters: None
    //Returns: None
    public virtual void RightPunch()
    {
        Punch(false);
    }

    //Punch
    //Description: Starts the punch animation for the designated fist
    //Parameters:
    //  left (bool): indicates left or right fist
    //Returns: None
    private void Punch(bool left)
    {
        //If not punching or dodging
        if (!isPunching && !isDodging)
        {
            //set isPunching to true and start the punch counter
            isPunching = true;
            punchTime = PUNCHMAX;

            //get the correct transform and move the position forward 
            Transform t;
            if (left)
                t = this.transform.GetChild(0);
            else
                t = this.transform.GetChild(1);

            //set local position of glove to default + punchDistance
            t.localPosition = t.localPosition + PUNCH;
        }
    }

    //LeftDodge
    //Description: Calls dodge for the left direction
    //Parameters: none
    //Returns: none
    public virtual void LeftDodge()
    {
        Dodge(LDODGELOC, LDODGE);
    }

    //RightDodge
    //Description: Calls dodge for the right direction
    //Parameters: none
    //Returns: none
    public virtual void RightDodge()
    {
        Dodge(RDODGELOC, RDODGE);
    }

    //BackDodge
    //Description: Calls dodge for the back direction
    //Parameters: none
    //Returns: none
    public virtual void BackDodge()
    {
        Dodge(BDODGELOC, BDODGE);
    }

    //Dodge
    //Description: Moves the boxer in the specified direction
    //Parameters:
    //  move (Vector3): the distance to move
    //  direction (int): number to set dodgeDirection to
    private void Dodge(Vector3 move, int direction)
    {
        if (!isDodging && !isPunching)
        {
            isDodging = true;
            dodgeDirection = direction;
            this.transform.localPosition = DEFAULT + (move * revMult);
        }
    }

    //Block
    //Description: moves the arms inward to the block position
    //Parameters: none
    //Returns: none
    public virtual void Block()
    {
        if (!isDodging && !isPunching)
        {
            isDodging = true;
            dodgeDirection = BLOCK;

            //get transforms of left arm and right arm
            Transform left = this.transform.GetChild(0);
            Transform right = this.transform.GetChild(1);

            //rotate both inward 45 degrees to show a blocking motion
            left.localEulerAngles = BLOCKANGLE * -1;
            right.localEulerAngles = BLOCKANGLE;
        }

    }


    //ResetDodges
    //Description: move the boxer to it's default position, un-cross arms
    //Parameters: none
    //Returns: none
    public void ResetDodges()
    {
        //reset dodge boolean
        isDodging = false;
        //set position to default
        this.transform.localPosition = DEFAULT;

        //if we were blocking, reset arm rotation to 0
        if (dodgeDirection == BLOCK)
        {
            Transform left = this.transform.GetChild(0);
            Transform right = this.transform.GetChild(1);

            left.localEulerAngles = new Vector3(0, 0, 0);
            right.localEulerAngles = new Vector3(0, 0, 0);
        }
        dodgeDirection = CENTER;
    }

    //ResetPunches
    //Description: moves both arms to their default positions
    //Parameters: none
    //Returns: none
    private void ResetPunches()
    {
        //bring both arms to a default position
        isPunching = false;

        Transform left = this.transform.GetChild(0);
        left.localPosition = LDEFAULT;

        Transform right = this.transform.GetChild(1);
        right.localPosition = RDEFAULT;
    }
}
