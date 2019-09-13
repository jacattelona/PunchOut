using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSwap : MonoBehaviour
{
    public Vector3 offenseLocation;
    public Vector3 defenseLocation;

    bool isSwapping = false;

    Vector3 target;
    Vector3 start;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSwapping)
        {

        }

        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {

            }
        }

    }
}
