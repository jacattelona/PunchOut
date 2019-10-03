using UnityEngine;
using System.Collections;

public class MyLog : MonoBehaviour
{
    string myLog;
    Queue myLogQueue = new Queue();
    int count = 0;

    void Start()
    {
        Debug.Log("Log1");
        Debug.Log("Log2");
        Debug.Log("Log3");
        Debug.Log("Log4");
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n\n [" + type + "] : " + myLog;
        myLogQueue.Enqueue(newString);
        count++;
        if (count >= 5)
        {
            myLogQueue.Dequeue();
        }
        if (type == LogType.Exception)
        {
            newString = "\n\n" + stackTrace;
            count++;
            if (count >= 5)
            {
                myLogQueue.Dequeue();
            }
            myLogQueue.Enqueue(newString);
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