using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A representation of the boxer sprite
/// </summary>
public class BoxerSprite : MonoBehaviour
{

    private Boxer boxer;

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

    private Renderer leftGloveRenderer, rightGloveRenderer, leftArmRenderer, rightArmRenderer;

    private Color gloveColor, armColor;

    private Vector3 DEFAULT = new Vector3(0, 0, 0);                  //Default position of the boxer

    // Start is called before the first frame update
    void Start()
    {
        boxer = GetComponent<Boxer>();
        DEFAULT = this.transform.Find("Sprite").localPosition;
        lastPunchState = boxer.GetPunchState();
        lastDodgeState = boxer.GetDodgeState();
        boxer.dodgeAction.animationStart.AddListener(StartDodgeAnimation);
        boxer.dodgeAction.animationEnd.AddListener(StopDodgeAnimation);
        boxer.punchAction.animationStart.AddListener(StartPunchAnimation);
        boxer.punchAction.animationEnd.AddListener(StopPunchAnimation);
        boxer.punchAction.action.AddListener(PunchAction);

        GameObject lg = this.transform.Find("Sprite").Find("LeftArm").Find("Glove").gameObject;
        leftGloveRenderer = lg.GetComponent<Renderer>();

        GameObject rg = this.transform.Find("Sprite").Find("RightArm").Find("Glove").gameObject;
        rightGloveRenderer = rg.GetComponent<Renderer>();

        GameObject la = this.transform.Find("Sprite").Find("LeftArm").Find("Forearm").gameObject;
        leftArmRenderer = la.GetComponent<Renderer>();

        GameObject ra = this.transform.Find("Sprite").Find("RightArm").Find("Forearm").gameObject;
        rightArmRenderer = ra.GetComponent<Renderer>();

        gloveColor = rightGloveRenderer.material.color;
        armColor = rightArmRenderer.material.color;
    }

    private void StartDodgeAnimation(int direction)
    {     
        if (direction == 1)
        {
            this.transform.Find("Sprite").localPosition = DEFAULT + LDODGELOC;
        } else
        {
            this.transform.Find("Sprite").localPosition = DEFAULT + RDODGELOC;
        }
    }

    private void StopDodgeAnimation(int direction)
    {
        this.transform.Find("Sprite").localPosition = DEFAULT;
        Transform left = this.transform.Find("Sprite").Find("LeftArm");
        Transform right = this.transform.Find("Sprite").Find("RightArm");

        left.localEulerAngles = new Vector3(0, 0, 0);
        right.localEulerAngles = new Vector3(0, 0, 0);
    }

    private void StartPunchAnimation(int side)
    {
        if (!boxer.broadcastPunch) return;
        Color broadcastColor = Color.red;
        if (side == 1)
        {
            leftGloveRenderer.material.color = broadcastColor;
            leftArmRenderer.material.color = broadcastColor;
        } else
        {
            rightGloveRenderer.material.color = broadcastColor;
            rightArmRenderer.material.color = broadcastColor;
        }
    }

    private void PunchAction(int side)
    {
        if (side == 1)
        {
            Transform t = this.transform.Find("Sprite").Find("LeftArm");
            leftGloveRenderer.material.color = gloveColor;
            leftArmRenderer.material.color = armColor;
            t.localPosition = t.localPosition + PUNCH;
        }
        else
        {
            Transform t = this.transform.Find("Sprite").Find("RightArm");
            rightGloveRenderer.material.color = gloveColor;
            rightArmRenderer.material.color = armColor;
            t.localPosition = t.localPosition + PUNCH;
        }
    }

    private void StopPunchAnimation(int side)
    {
        Transform left = this.transform.Find("Sprite").Find("LeftArm");
        left.localPosition = LDEFAULT;

        Transform right = this.transform.Find("Sprite").Find("RightArm");
        right.localPosition = RDEFAULT;

        leftGloveRenderer.material.color = gloveColor;
        rightGloveRenderer.material.color = gloveColor;
        leftArmRenderer.material.color = armColor;
        rightArmRenderer.material.color = armColor;
    }

    // Update is called once per frame
    void FixedUpdate()
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
