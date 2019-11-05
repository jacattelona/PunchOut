using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public int state;
    public GameHandler gameHand;
    public Animator anim;
    public GameObject intro1;
    public GameObject intro2;
    public GameObject punch1Set1;
    public GameObject punch1Set2;
    public GameObject punchSet21;
    public GameObject punchSet22;
    
    // Start is called before the first frame update
    void Start()
    {
        state = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("a") && state == 1)
        {
            state++;
            intro1.SetActive(false);
            intro2.SetActive(false);
            anim.SetTrigger("gotoPunch1Setup");
            
        }
        else if(Input.GetKeyDown("k") && state == 5)
        {
            anim.SetTrigger("gotoDodge2");
            state++;
        }
        else if (Input.GetKeyDown("d") && state == 4)
        {
            anim.SetTrigger("gotoDodge1");
            state++;
        }
        else if (Input.GetKeyDown("space") && state == 6)
        {
            anim.SetTrigger("gotoExplainOff");
            state++;
        }
        else if (Input.GetKeyDown("d") && state == 7)
        {
            gameHand.StartOffensive();
            state += 10;
        }
        else if (Input.GetKeyDown("d") && state == 8)
        {
            anim.SetTrigger("gotoExplainTourney");
            state++;
        }
        else if (Input.GetKeyDown("space") && state == 9)
        {
            anim.SetTrigger("gotoExit");
            //SceneManager.LoadScene("scene1");
        }
        else if (Input.GetKeyDown("f") && state == 2)
        {
            punch1Set1.SetActive(false);
            anim.SetTrigger("gotoPunch1");
            state++;
        }
        else if (Input.GetKeyDown("j") && state == 3)
        {
            //punch1Set1.SetActive(false);
            anim.SetTrigger("gotoPunch2");
            state++;
        }
    }
}
