using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimpleButton : MonoBehaviour
{
    public string nextLevel;
    public Button a;
    // Start is called before the first frame update
    void Start()
    {
        a.onClick.AddListener(checkLevel);
    }

    void checkLevel()
    {
        //if (spirite1.activeSelf && spirite1.activeSelf && spirite1.activeSelf)
        //{
        //    SceneManager.LoadScene(nextLevel);
        //}
        SceneManager.LoadScene(nextLevel);
    }
}
