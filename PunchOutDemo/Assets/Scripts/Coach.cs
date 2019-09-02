using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coach : MonoBehaviour
{

    public GameObject player;
    public KeyCode positiveKey, negativeKey;

    public float rewardStrength = 1f;

    public string positiveMessage = "Good job, keep it up!";
    public string negativeMessage = "Don't do that again!";

    private Boxer playerBoxer;
    public Text display;

    private float displayTimeout = 1f;
    private float lastDisplayUpdate = -1f;


    // Start is called before the first frame update
    void Start()
    {
        playerBoxer = player.GetComponent<Boxer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(negativeKey))
        {
            playerBoxer.AddReward(-rewardStrength);
            display.text = negativeMessage;
            lastDisplayUpdate = Time.fixedTime;
        }
        else if (Input.GetKeyDown(positiveKey))
        {
            playerBoxer.AddReward(rewardStrength);
            display.text = positiveMessage;
            lastDisplayUpdate = Time.fixedTime;
        }

        if (Time.fixedTime - lastDisplayUpdate >= displayTimeout)
        {
            display.text = "";
        }
    }
}
