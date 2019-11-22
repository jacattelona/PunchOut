using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public Text textOb;
    public TutorialManager manager;
    public SpriteRenderer render;
    public int State;
    public bool start = false;
    public bool image;
    //public float timeIncrement;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (image && start == false)
        {
            StartCoroutine("fadeOut");
            start = true;
        }
        if (manager.state == State && start == false)
        {
            start = true;
            StartCoroutine("fadeOu");
            
        }
    }
    IEnumerator fadeOu()
    {
        for (float f = 1f; f > -0.05f; f -= .05f)
        {
            Color a = textOb.color;
            a.a = f;
            textOb.color = a;
            yield return new WaitForSeconds(.05f);
        }
    }
    IEnumerator fadeOut()
    {
        for (float f = 1f; f > -0.05f; f -= .05f)
        {
            Color a = render.color;
            a.a = f;
            render.color = a;
            yield return new WaitForSeconds(.05f);
        }
    }
}
