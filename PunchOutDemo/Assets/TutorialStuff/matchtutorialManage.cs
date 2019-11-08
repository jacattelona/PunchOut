using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class matchtutorialManage : MonoBehaviour
{
    public GameHandler handle;
    public Animator anim;
    public Button fight;
    public Button train;

    // Start is called before the first frame update
    void Start()
    {
        anim.SetTrigger("trigger1");
        fight.onClick.AddListener(goFight);
        train.onClick.AddListener(goTrain);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown("t"))
        {
            handle.StartOffensive();
        }
        else if (Input.GetKeyDown("f"))
        {
            handle.setState(6);
        }
    }

    public void goTrain()
    {
        handle.StartOffensive();
    }

    public void goFight()
    {
        handle.setState(6);
    }

    
}
