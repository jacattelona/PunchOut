using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSwap : MonoBehaviour
{
    public Vector3 offenseLocation;
    public Vector3 defenseLocation;
    public Vector3 fightLocation;
    public UnityEngine.UI.Text timer;
    public bool oneScene = true;
    public GameObject switchButton;
    Vector3 velocity = Vector3.zero;

    bool isSwapping = false;
    float t = 0;

    Location loc;

    public enum Location
    {
        Offense,
        Defense,
        Fight
    }


    // Start is called before the first frame update
    void Start()
    {
        transform.position = offenseLocation;
        loc = Location.Offense;
        t = 10f;
        timer.text = "Time Left: " + Mathf.Ceil(t);
    }

    // Update is called once per frame
    void Update()
    {
        if (isSwapping)
        {
            //t += Time.deltaTime * .5f;
            //if (t > 1.0f)
            //{
            //    t = 1.0f;
            //    isSwapping = false;
            //}
            //transform.position = Vector3.Lerp(offenseLocation, defenseLocation, t);
            if (loc == Location.Offense)
            {
                if (Vector3.Distance(transform.position, defenseLocation) < .01f)
                {
                    isSwapping = false;
                    loc = Location.Defense;
                    t = 10f;
                }

                transform.position = Vector3.SmoothDamp(transform.position, defenseLocation, ref velocity, 1f);
            }

            if (loc == Location.Defense)
            {
                if (Vector3.Distance(transform.position, fightLocation) < .01f)
                {
                    isSwapping = false;
                    loc = Location.Fight;
                    t = 60;
                }

                transform.position = Vector3.SmoothDamp(transform.position, fightLocation, ref velocity, 1f);
            }


        }

        else
        {
            //if (Input.GetKeyDown(KeyCode.Alpha1))
            //{
            //    isSwapping = true;
            //}
            if (oneScene || loc != Location.Fight)
            {
                t -= Time.deltaTime;
                timer.text = "Time Left: " + Mathf.Ceil(t);

                if (t <= 0)
                    isSwapping = true;
            }

            else if (loc == Location.Fight)
            {
                switchButton.SetActive(true);
            }

        }

    }
}
