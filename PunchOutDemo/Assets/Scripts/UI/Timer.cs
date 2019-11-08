using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    private Transform needle;
    private TextMeshPro timeTxt;
    private Image progressCircle;

    public float maxTime = 1;

    private float timeLeft;
    private bool running = false;

    void Start()
    {
        needle = transform.Find("Needle");
        timeTxt = transform.Find("Time").GetComponent<TextMeshPro>();
        progressCircle = transform.Find("Progress Circle").GetComponentInChildren<Image>();
        ResetTimer();
        StartTimer();
    }

    private void Update()
    {
        if (!running) return;

        // Update the time left
        timeLeft -= Time.deltaTime;

        if (timeLeft < 0)
        {
            TimerExpired();
            return;
        }

        needle.eulerAngles = new Vector3(0, 0, (1 - GetTimePercent()) * 360f - 90f);
        progressCircle.fillAmount = 1 - GetTimePercent();

        int minutes = Mathf.FloorToInt(timeLeft) / 60;
        int seconds = Mathf.FloorToInt(timeLeft - minutes * 60);

        // Display the time left
        timeTxt.text = String.Format("{0:D1}:{1:D2}", minutes, seconds);
    }

    private float GetTimePercent()
    {
        return Mathf.Clamp01(timeLeft / maxTime);
    }

    /// <summary>
    /// Handles the timer expired logic
    /// </summary>
    private void TimerExpired()
    {
        // TODO: Flash
    }

    /// <summary>
    /// Determines if the timer is expired
    /// </summary>
    /// <returns>True if the timer is expired</returns>
    public bool IsExpired()
    {
        return timeLeft <= 0;
    }

    /// <summary>
    /// Start the timer
    /// </summary>
    public void StartTimer()
    {
        running = true;
    }

    /// <summary>
    /// Stop the timer
    /// </summary>
    public void StopTimer()
    {
        running = false;
    }

    /// <summary>
    /// Resets the timer to it's starting state
    /// </summary>
    public void ResetTimer()
    {
        StopTimer();
        timeLeft = maxTime;
    }

   
}
