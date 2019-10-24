using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveLog : MonoBehaviour
{
    string myLog;
    Queue myLogQueue = new Queue();
    Queue myPics = new Queue();
    int count = 0;
    Texture2D LP, LD, RP, RD;
    


    public Boxer boxer;
    // Start is called before the first frame update
    void Start()
    {
        boxer = GetComponent<Boxer>();
        boxer.punchAction.action.AddListener(logPunch);
        boxer.dodgeAction.action.AddListener(logDodge);
        LP = Resources.Load("Key Logs/Left Punch") as Texture2D;
        LD = Resources.Load("Key Logs/Left Dodge") as Texture2D;
        RP = Resources.Load("Key Logs/Right Punch") as Texture2D;
        RD = Resources.Load("Key Logs/Right Dodge") as Texture2D;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void logPunch(int side)
    {
        if (side == 1)
        {
            //HandleLog("Left Punch");
            HandlePic(LP);
            
        }

        if (side == 2)
        {
          //  HandleLog("Right Punch");
            HandlePic(RP);
        }
    }

    void logDodge(int side)
    {
        if (side == 1)
        {
           // HandleLog("Left Dodge");
            HandlePic(LD);
        }

        if (side == 2)
        {
          //  HandleLog("Right Dodge");
            HandlePic(RD);
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

    void HandlePic(Texture tex)
    {
        
        myPics.Enqueue(tex);
        count++;
        if (count >= 10)
        {
            myPics.Dequeue();
        }

        
       
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(1750, 10, 100, 1200));
        
        // GUILayout.Label(myLog);
        foreach(Texture2D tx in myPics)
        {
            GUILayout.Label(tx);
        }

        GUILayout.EndArea();
        
    }

}
