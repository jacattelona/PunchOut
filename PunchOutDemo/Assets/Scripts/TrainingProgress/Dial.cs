using TMPro;
using UnityEngine;

public class Dial : TrainingProgress 
{

    private Transform needle;
    private TextMeshPro percentageTxt;

    private const float MIN_ANGLE = 18.0f;
    private const float MAX_ANGLE = -197.0f;

    public float changeSpeed = 2f;

    private float progressAngle = 0;
    private float progress = 0;

    void Start()
    {
        needle = transform.Find("Needle");
        percentageTxt = transform.Find("Percentage").GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        var diff = progressAngle - progress;
        if (Mathf.Abs(diff) < 0.05)
        {
            progressAngle = GetProgress();
        }
        else
        {
            progressAngle -= diff * changeSpeed * Time.deltaTime;
        }
        var range = Mathf.Abs(MAX_ANGLE - MIN_ANGLE);
        var angle = MIN_ANGLE - progressAngle * range;
        percentageTxt.text = Mathf.RoundToInt(progressAngle * 100).ToString() + " %";
        needle.eulerAngles = new Vector3(0, 0, angle);
    }

    public override void Clear()
    {
        SetProgress(0);
    }

    public override float GetProgress()
    {
        return progress;
    }

    public override void SetProgress(float progress)
    {
        this.progress = Mathf.Clamp01(progress);
    }
}
