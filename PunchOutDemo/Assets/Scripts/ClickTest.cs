using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickTest : MonoBehaviour
{
    public Button buttin;
    public bool clicked;
    public bool two;
    public GameObject shown;
    public GameObject sword1;
    public GameObject shield;
    public GameObject sword;
    public GameObject twoSwords;
    public GameObject last;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = buttin.GetComponent<Button>();
        btn.onClick.AddListener(onClick);
    }

    public void onClick()
    {
        shown.SetActive(true);
        if (two)
        {
            sword1.SetActive(true);
        }
        
        if (shield.activeSelf && sword.activeSelf && twoSwords.activeSelf)
        {
            last.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
