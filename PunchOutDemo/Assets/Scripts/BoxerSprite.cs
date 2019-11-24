using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A representation of the boxer sprite
/// </summary>
public class BoxerSprite : MonoBehaviour
{
    private Boxer2 boxer;
    private BoxerAudio boxerAudio;

    private Renderer leftGloveRenderer, rightGloveRenderer, bodyRenderer;

    private Color gloveColor;

    float damageTime = 0;
    float maxDamageTime = .25f;

    // Start is called before the first frame update
    void Start()
    {
        boxer = GetComponent<Boxer2>();

        boxer.hitEvent.AddListener(ShowDamage);

        GameObject lg = this.transform.Find("Sprite").Find("LeftArm").gameObject;
        leftGloveRenderer = lg.GetComponent<Renderer>();

        GameObject rg = this.transform.Find("Sprite").Find("RightArm").gameObject;
        rightGloveRenderer = rg.GetComponent<Renderer>();

        bodyRenderer = this.transform.Find("Sprite").Find("Body").GetComponent<Renderer>();

        gloveColor = rightGloveRenderer.material.color;

        boxerAudio = GetComponent<BoxerAudio>();
    }

    public void ShowDamage()
    {
        damageTime = maxDamageTime;
        bodyRenderer.material.color = Color.red;
        
        //print("Invoked");
    }

    // Update is called once per frame
    void Update()
    {
        if (damageTime > 0)
        {
            damageTime -= Time.deltaTime;

            if (damageTime <= 0)
                bodyRenderer.material.color = gloveColor;
        }
    }
}
