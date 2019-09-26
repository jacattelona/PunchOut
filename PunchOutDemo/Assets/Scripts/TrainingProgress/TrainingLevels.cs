using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingLevels : TrainingProgress
{

    public float value;
    private float lastLevel;

    public float level1Threshold, level2Threshold, level3Threshold, level4Threshold, level5Threshold;
    public string lowLabel, mediumLabel, highLabel;

    [SerializeField]
    private Sprite lowSprite, mediumSprite, highSprite, offSprite;

    private SpriteRenderer[] spriteRenderers;

    private Text lowText, mediumText, highText;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderers = new SpriteRenderer[]
        {
               transform.Find("Level1").GetComponent<SpriteRenderer>(),
               transform.Find("Level2").GetComponent<SpriteRenderer>(),
               transform.Find("Level3").GetComponent<SpriteRenderer>(),
               transform.Find("Level4").GetComponent<SpriteRenderer>(),
               transform.Find("Level5").GetComponent<SpriteRenderer>(),
               transform.Find("Level6").GetComponent<SpriteRenderer>()
        };

        lowText = transform.Find("Low").Find("Text").GetComponent<Text>();
        mediumText = transform.Find("Medium").Find("Text").GetComponent<Text>();
        highText = transform.Find("High").Find("Text").GetComponent<Text>();

        DrawLevel();
        lastLevel = GetLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (lastLevel != GetLevel())
        {
            DrawLevel();
            lastLevel = GetLevel();
        }

        lowText.text = lowLabel;
        mediumText.text = mediumLabel;
        highText.text = highLabel;
    }

    private int GetLevel()
    {
        if (value < level1Threshold)
        {
            return 0;
        } else if (value < level2Threshold)
        {
            return 1;
        } else if (value < level3Threshold)
        {
            return 2;
        } else if (value < level4Threshold)
        {
            return 3;
        } else if (value < level5Threshold)
        {
            return 4;
        } else
        {
            return 5;
        }
    }

    private void DrawLevel()
    {
        int level = GetLevel();
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (i <= level)
            {
                ActivateLevel(i);
            } else
            {
                DeactivateLevel(i);
            }
        }
    }

    private void DeactivateLevel(int level)
    {
        spriteRenderers[level].sprite = offSprite;
    }

    private void ActivateLevel(int level)
    {
        if (level == 0 || level == 1)
        {
            spriteRenderers[level].sprite = lowSprite;
        } else if (level == 2 || level == 3)
        {
            spriteRenderers[level].sprite = mediumSprite;
        } else
        {
            spriteRenderers[level].sprite = highSprite;
        }

    }

    public override void SetProgress(float progress)
    {
        this.value = Mathf.Clamp(progress, 0, 1);
    }

    public override float GetProgress()
    {
        return value;
    }

    public override void Clear()
    {
        SetProgress(0);
    }
}
