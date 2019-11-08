using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTimeline : MonoBehaviour
{

    
    List<GameObject> iconList;
    


    // Start is called before the first frame update
    void Start()
    {
        iconList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (var thought in iconList)
        //{
          //  thought.transform.position = new Vector3(thought.transform.position.x, thought.transform.position.y + .1f, thought.transform.position.z);
            
       // }
       this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + .1f, this.gameObject.transform.position.z);
        if (this.gameObject.transform.position.y >= 12f)
        {
            Destroy(this.gameObject);
            Debug.Log("Destroyed");
        }
    }

   

    //Instantiate(myPrefab, new Vector3(1000, 1000, 0), Quaternion.identity);
}
