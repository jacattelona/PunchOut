using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A representation of the boxer sprite
/// </summary>
public class BoxerSprite : MonoBehaviour
{
    Animator anim;

    private Boxer boxer;

    //Const Values (you can't declare Vector3s const, so just pretend)
    protected Vector3 LDEFAULT = new Vector3(-1, 0, 0);             //Default position of the left glove
    protected Vector3 RDEFAULT = new Vector3(1, 0, 0);              //Default position of the right glove
    protected Vector3 PUNCH = new Vector3(0, 1.7f, 0);                 //Distance a glove moves forward during punch

    public float dodgeDistance = 3;

    Vector3 LDODGELOC;                     //Number of units to move when dodging left
    Vector3 RDODGELOC;                     //Number of units to move when dodging right
    Vector3 BDODGELOC = new Vector3(0, -2, 0);                      //Number of units to move when dodging back
    Vector3 BLOCKANGLE = new Vector3(0, 0, 45);                     //Angle to move arms inward when blocking

    protected int punchTime = 0;                                    //number of frames left in punch animation
    private Punch lastPunchState;
    private DodgeState lastDodgeState;

    private Renderer leftGloveRenderer, rightGloveRenderer, bodyRenderer;

    private Color gloveColor;

    private Vector3 DEFAULT = new Vector3(0, 0, 0);                  //Default position of the boxer

    float damageTime = 0;
    float maxDamageTime = .25f;

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

        boxer.hitEvent.AddListener(ShowDamage);

        GameObject lg = this.transform.Find("Sprite").Find("LeftArm").gameObject;
        leftGloveRenderer = lg.GetComponent<Renderer>();

        GameObject rg = this.transform.Find("Sprite").Find("RightArm").gameObject;
        rightGloveRenderer = rg.GetComponent<Renderer>();

        bodyRenderer = this.transform.Find("Sprite").Find("Body").GetComponent<Renderer>();

        gloveColor = rightGloveRenderer.material.color;

        LDODGELOC = new Vector3(-dodgeDistance, 0, 0);
        RDODGELOC = new Vector3(dodgeDistance, 0, 0);

        anim = GetComponent<Animator>();
    }

    private void StartDodgeAnimation(int direction)
    {     
        if (direction == 1)
        {
            //this.transform.Find("Sprite").localPosition = DEFAULT + LDODGELOC;
            anim.Play("DodgeLeft");
        } else
        {
            //this.transform.Find("Sprite").localPosition = DEFAULT + RDODGELOC;
            anim.Play("DodgeRight");
        }
    }

    private void StopDodgeAnimation(int direction)
    {
        this.transform.Find("Sprite").localPosition = DEFAULT;
        //Transform left = this.transform.Find("Sprite").Find("LeftArm");
        //Transform right = this.transform.Find("Sprite").Find("RightArm");

        //left.localEulerAngles = new Vector3(0, 0, 0);
        //right.localEulerAngles = new Vector3(0, 0, 0);
    }

    private void StartPunchAnimation(int side)
    {
        if (!boxer.broadcastPunch) return;
        Color broadcastColor = Color.red;
        anim.Play("None");
        if (side == 1)
        {
            leftGloveRenderer.material.color = broadcastColor;
            tel = Telegraphing.PunchLeft;
        } else
        {
            rightGloveRenderer.material.color = broadcastColor;
            tel = Telegraphing.PunchRight;
        }
    }

    private void PunchAction(int side)
    {
        tel = Telegraphing.None;
        intensity = 0;
        if (side == 1)
        {
            Transform t = this.transform.Find("Sprite").Find("LeftArm");
            t.localPosition = LDEFAULT;
            leftGloveRenderer.material.color = gloveColor;
            //t.localPosition = t.localPosition + PUNCH;
            anim.Play("PunchLeft");
        }
        else
        {
            Transform t = this.transform.Find("Sprite").Find("RightArm");
            t.localPosition = RDEFAULT;
            rightGloveRenderer.material.color = gloveColor;
            anim.Play("PunchRight");
            //t.localPosition = t.localPosition + PUNCH;
        }
    }

    private void StopPunchAnimation(int side)
    {
        Transform left = this.transform.Find("Sprite").Find("LeftArm");
        //left.localPosition = LDEFAULT;

        Transform right = this.transform.Find("Sprite").Find("RightArm");
        //right.localPosition = RDEFAULT;

        leftGloveRenderer.material.color = gloveColor;
        rightGloveRenderer.material.color = gloveColor;
    }

    public void ShowDamage()
    {
        damageTime = maxDamageTime;
        bodyRenderer.material.color = Color.red;
        //print("Invoked");
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

        if (damageTime > 0)
        {
            damageTime -= Time.deltaTime;

            if (damageTime <= 0)
                bodyRenderer.material.color = gloveColor;
        }
    }
}
