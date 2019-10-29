using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    float t;
    Vector3 startPosition;
    Vector3 target;
    float timeToTarget;


    // Start is called before the first frame update
    void Start()
    {
        startPosition = target = transform.position;

        target = new Vector3(1750, -100, 0);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime / timeToTarget;
        transform.position = Vector3.Lerp(startPosition, target, t);

    }

    public void SetDestination(Vector3 dest, float time)
    {
        t = 0;
        startPosition = transform.position;
        timeToTarget = time;
        target = dest;
    }
}
