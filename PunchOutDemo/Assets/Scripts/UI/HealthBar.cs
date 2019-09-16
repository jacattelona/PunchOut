using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private Health health;
    private Transform bar;

    private int lastHealth = -1;

    // Start is called before the first frame update
    void Start()
    {

        health = GetComponent<Health>();
        bar = transform.Find("HealthBar").Find("Bar");        
    }

    // Update is called once per frame
    void Update()
    {
        if (lastHealth == health.health)
        {
            return;
        }
        float healthPct = health.health / (float)health.maxHealth;
        bar.localScale = new Vector3(healthPct, 1f);
        if (healthPct <= 0.3f)
        {
            SetBarColor(new Color(255, 0, 0));
        } else if (healthPct <= 0.6f)
        {
            SetBarColor(new Color(255, 255, 0));
        } else
        {
            SetBarColor(new Color(0, 255, 0));
        }

        lastHealth = health.health;
    }

    private void SetBarColor(Color color)
    {
        bar.Find("BarSprite").GetComponent<SpriteRenderer>().color = color;
    }
}
