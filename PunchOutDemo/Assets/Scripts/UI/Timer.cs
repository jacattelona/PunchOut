using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    private TextMeshPro timeTxt;

    public float maxTime = 1;

    private float timeLeft;
    private bool running = false;

    void Start()
    {
        timeTxt = transform.Find("Time").GetComponent<TextMeshPro>();
        ResetTimer();
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
        } else
        {
            timeTxt.color = new Color(1, 1, 1);
        }

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
        timeTxt.color = new Color(1, 0, 0);
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
