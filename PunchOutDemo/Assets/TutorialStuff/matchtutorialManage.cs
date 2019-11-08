using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class matchtutorialManage : MonoBehaviour
{
    public Animator anim;
    public Button fight;
    public Button train;

    public UnityEvent retrain, match;

    // Start is called before the first frame update
    void Start()
    {
        anim.SetTrigger("trigger1");
        retrain = new UnityEvent();
        match = new UnityEvent();
        fight.onClick.AddListener(goFight);
        train.onClick.AddListener(goTrain);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            goTrain();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            goFight();
        }
    }

    public void goTrain()
    {
        retrain.Invoke();
    }

    public void goFight()
    {
        match.Invoke();
    }

    
}
