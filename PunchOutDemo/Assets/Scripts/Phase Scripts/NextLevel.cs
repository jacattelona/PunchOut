using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    // Start is called before the first frame update
    public string nextLevel;
    public Button a;
    
    public GameObject spirite1;
    public GameObject spirite2;
    public GameObject spirite3;
    void Start()
    {
        a.onClick.AddListener(checkLevel);
    }

    // Update is called once per frame
    void Update()
    {
        if (spirite1.activeSelf && spirite1.activeSelf && spirite1.activeSelf)
        {

        }
    }

    void checkLevel()
    {
        if (spirite1.activeSelf && spirite1.activeSelf && spirite1.activeSelf)
        {
            SceneManager.LoadScene(nextLevel);
        }

    }

}
