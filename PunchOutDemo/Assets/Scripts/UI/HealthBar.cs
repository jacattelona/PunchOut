using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{

    private Boxer2 boxer;

    private Transform bar;

    private float lastHealth = -1;

    // Start is called before the first frame update
    void Start()
    {

        boxer = GetComponent<Boxer2>();
        bar = transform.Find("HealthBar").Find("Bar");        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Mathf.Approximately(lastHealth, boxer.hp))
        {
            return;
        }
        float healthPct = boxer.hp/ (float)boxer.maxHP;
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

        lastHealth = boxer.hp;
    }

    private void SetBarColor(Color color)
    {
        bar.Find("BarSprite").GetComponent<SpriteRenderer>().color = color;
    }
}
