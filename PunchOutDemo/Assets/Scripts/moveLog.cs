using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveLog : MonoBehaviour
{
    string myLog;
    Queue myLogQueue = new Queue();
    int count = 0;


    public Boxer boxer;
    // Start is called before the first frame update
    void Start()
    {
        boxer = GetComponent<Boxer>();
        boxer.punchAction.action.AddListener(logPunch);
        boxer.dodgeAction.action.AddListener(logDodge);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void logPunch(int side)
    {
        if (side == 1)
        {
            HandleLog("Left Punch");
        }

        if (side == 2)
        {
            HandleLog("Right Punch");
        }
    }

    void logDodge(int side)
    {
        if (side == 1)
        {
            HandleLog("Left Dodge");
        }

        if (side == 2)
        {
            HandleLog("Right Dodge");
        }
    }

   


  

    void HandleLog(string logString)
    {
        myLog = logString;
        string newString = "\n\n" + myLog;
        myLogQueue.Enqueue(newString);
        count++;
        if (count >= 5)
        {
            myLogQueue.Dequeue();
        }
        
        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }
    }

    void OnGUI()
    {
        GUILayout.Label(myLog);
    }

}
