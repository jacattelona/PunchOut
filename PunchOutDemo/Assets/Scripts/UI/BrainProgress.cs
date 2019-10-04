using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BrainProgress : TrainingProgress
{

    public float progress = 0;
    public bool hasProgress = false;

    public string noValueText = "N/A";

    public float fillSpeed = 2f;

    [SerializeField]
    private Color emptyColor, fullColor;

    [SerializeField]
    private TextMeshPro text;

    [SerializeField]
    private Transform bar;

    private SpriteRenderer barSpriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        barSpriteRenderer = bar.gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasProgress)
        {
            float scale = bar.localScale.y;
            float diff = scale - GetProgress();
            if (Mathf.Abs(diff) < 0.01)
            {
                scale = GetProgress();
            } else
            {
                scale -= diff * fillSpeed * Time.deltaTime;
            }
            text.text = (scale * 100).ToString("0") + " %";
            bar.localScale = new Vector3(1, scale);
            barSpriteRenderer.color = GetCurrentColor();
        } else
        {
            text.text = noValueText;
            bar.localScale = new Vector3(1, 0);
        }
    }

    private Color GetCurrentColor()
    {
        float p = GetProgress();
        float scale = bar.localScale.y;
        Color lerped;
        if (p < scale)
        {
            lerped = Color.Lerp(fullColor, emptyColor, 1 - scale);
        } else
        {
            lerped = Color.Lerp(emptyColor, fullColor, scale);
        }
        
        return new Color(lerped.r, lerped.g, lerped.b, 1);
    }

    private float ToRange(float value, float min, float max)
    {
        if (min == max) return max;
        float range = max - min;
        return value * range + min;
    }


    public override void SetProgress(float progress)
    {
        this.progress = progress;
        this.hasProgress = true;
    }

    public override float GetProgress()
    {
        return Mathf.Clamp01(progress);
    }

    public override void Clear()
    {
        this.progress = 0;
    }
}
