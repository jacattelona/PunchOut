using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIN : MonoBehaviour
{
    public Text textOb;
    public TutorialManager manager;
    public int State;
    public SpriteRenderer render;
    public bool image;
    public bool start = false;
    //public float timeIncrement;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (image && !start && manager.state == State)
        {
            start = true;
            StartCoroutine("fadeI");
        }
        if (manager.state == State && start == false && !image)
        {
            start = true;
            StartCoroutine("fadeIn");

        }
    }
    IEnumerator fadeIn()
    {
        for (float f = 0f; f < 1f; f += .05f)
        {
            Color a = textOb.color;
            a.a = f;
            textOb.color = a;
            yield return new WaitForSeconds(.05f);
        }
    }

    IEnumerator fadeI()
    {
        for (float f = 0f; f < 1f; f += .05f)
        {
            Color a = render.color;
            a.a = f;
            render.color = a;
            yield return new WaitForSeconds(.05f);
        }
    }
}
