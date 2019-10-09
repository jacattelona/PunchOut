using System.Collections.Generic;
using UnityEngine;

public class ActionHistory
{
    public struct MLActionEvent
    {
        public MLAction action;
        public float time;
    }

    private List<MLActionEvent> actions;

    public ActionHistory()
    {
        actions = new List<MLActionEvent>();
    }

    public void Record(MLAction action)
    {
        actions.Add(new MLActionEvent { action = action, time = Time.time });
    }

    public MLActionEvent GetLastAction()
    {
        if (actions.Count == 0) return new MLActionEvent { action = MLAction.NOTHING, time = Time.time };
        return actions[actions.Count - 1];
    }

    public List<MLActionEvent> GetActions()
    {
        return actions;
    }
}
