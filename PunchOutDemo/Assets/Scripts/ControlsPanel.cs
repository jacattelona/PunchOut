using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsPanel : MonoBehaviour
{

    [SerializeField]
    public Boxer boxer;

    public bool reactive = true;

    private Color buttonPressed, buttonUnpressed;

    private GameObject leftPunch, rightPunch, leftDodge, rightDodge, viewAI;

    // Start is called before the first frame update
    void Start()
    {
        leftPunch = transform.Find("Left Punch").gameObject;
        rightPunch = transform.Find("Right Punch").gameObject;
        leftDodge = transform.Find("Left Dodge").gameObject;
        rightDodge = transform.Find("Right Dodge").gameObject;
        viewAI = transform.Find("View AI").gameObject;

        buttonPressed = new Color(1, 1, 1, 1);
        buttonUnpressed = new Color(1, 1, 1, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

        if (!reactive)
        {
            SetEnabled(leftPunch);
            SetEnabled(rightPunch);
            SetEnabled(leftDodge);
            SetEnabled(rightDodge);

            SetButtonColor(leftPunch, buttonUnpressed);
            SetButtonColor(rightPunch, buttonUnpressed);
            SetButtonColor(leftDodge, buttonUnpressed);
            SetButtonColor(rightDodge, buttonUnpressed);
            return;
        }

        MLAction action = boxer.currentAction;
        bool punchCooldown = boxer.punchAction.IsOnCooldown();
        bool dodgeCooldown = boxer.dodgeAction.IsOnCooldown();

        SetButtonColor(leftPunch, Input.GetKey(KeyCode.F) ? buttonPressed : buttonUnpressed);
        SetButtonColor(rightPunch, Input.GetKey(KeyCode.J) ? buttonPressed : buttonUnpressed);
        SetButtonColor(leftDodge, Input.GetKey(KeyCode.D) ? buttonPressed : buttonUnpressed);
        SetButtonColor(rightDodge, Input.GetKey(KeyCode.K) ? buttonPressed : buttonUnpressed);

        if (action != MLAction.NOTHING)
        {
            SetDisabled(leftPunch);
            SetDisabled(rightPunch);
            SetDisabled(leftDodge);
            SetDisabled(rightDodge);
        } else
        {
            SetEnabled(leftPunch);
            SetEnabled(rightPunch);
            SetEnabled(leftDodge);
            SetEnabled(rightDodge);
        }

        if (punchCooldown)
        {
            SetDisabled(leftPunch);
            SetDisabled(rightPunch);
        }

        if (dodgeCooldown)
        {
            SetDisabled(leftDodge);
            SetDisabled(rightDodge);
        }

    }

    void SetButtonColor(GameObject action, Color buttonColor)
    {
        action.GetComponent<Image>().color = buttonColor;
    }

    void SetDisabled(GameObject action)
    {
        action.transform.Find("Action Icon Template").Find("Background").GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        action.transform.Find("Action Icon Template").Find("Image").GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
    }

    void SetEnabled(GameObject action)
    {
        action.transform.Find("Action Icon Template").Find("Background").GetComponent<Image>().color = new Color(1, 1, 1, 0.8f);
        action.transform.Find("Action Icon Template").Find("Image").GetComponent<Image>().color = new Color(1, 1, 1, 1f);
    }

}
