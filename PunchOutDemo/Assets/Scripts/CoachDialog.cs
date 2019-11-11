using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoachDialog : MonoBehaviour
{
    public static CoachDialog instance;

    private TextMeshProUGUI mText;
    private float lastShowDuration = float.PositiveInfinity;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        Hide();
        Clear();
    }

    private void Update()
    {
        lastShowDuration -= Time.deltaTime;
        if (lastShowDuration <= 0)
        {
            Clear();
            Hide();
        }
    }

    /// <summary>
    /// Clear the text without hiding the dialog
    /// </summary>
    public void Clear()
    {
        mText.text = "";
    }

    /// <summary>
    /// Show some text on the dialog
    /// </summary>
    /// <param name="text">The text to show</param>
    public void Show(string text)
    {
        Show(text, float.PositiveInfinity);
    }

    /// <summary>
    /// Show some text on the dialog
    /// </summary>
    /// <param name="text">The text to show</param>
    /// <param name="duration">The duration to show for in seconds</param>
    public void Show(string text, float duration)
    {
        mText.text = text;
        if (text == "")
        {
            Hide();
        }
        else
        {
            gameObject.SetActive(true);
            lastShowDuration = duration;
        }
    }

    /// <summary>
    /// Hide the dialog
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
