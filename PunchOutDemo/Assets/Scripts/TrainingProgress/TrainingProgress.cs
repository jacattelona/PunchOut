using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrainingProgress: MonoBehaviour
{
    public abstract void SetProgress(float progress);

    public abstract float GetProgress();

    public abstract void Clear();

    public void SetEnabled(bool enabled)
    {
        gameObject.SetActive(enabled);
    }

    public bool IsEnabled()
    {
        return gameObject.activeInHierarchy;
    }
}
