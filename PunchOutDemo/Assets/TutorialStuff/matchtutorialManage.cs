using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class matchtutorialManage : MonoBehaviour
{
    public GameHandler handle;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim.SetTrigger("trigger1");
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
}
