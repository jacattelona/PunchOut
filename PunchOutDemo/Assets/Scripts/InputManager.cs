using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    //singleton
    public static InputManager Instance;

    //Events that are fired based on keyboard input
    public UnityEvent LeftPunch;
    public UnityEvent RightPunch;
    public UnityEvent LeftDodge;
    public UnityEvent RightDodge;
    public UnityEvent BackDodge;
    public UnityEvent Block;
    public UnityEvent Reset;

    void Awake()
    {
        //instantiate singleton
        if (Instance != null && Instance != this)
            Destroy(this);

        else
            Instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        //create the events
        LeftPunch = new UnityEvent();
        RightPunch = new UnityEvent();
        LeftDodge = new UnityEvent();
        RightDodge = new UnityEvent();
        BackDodge = new UnityEvent();
        Block = new UnityEvent();
        Reset = new UnityEvent();

    }

    // Update is called once per frame
    void Update()
    {
        //Left click
        if (Input.GetMouseButtonDown(0))
        {
            LeftPunch.Invoke();
        }
        //Right Click
        if (Input.GetMouseButtonDown(1))
        {
            RightPunch.Invoke();
        }

        //Left Arrow
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            LeftDodge.Invoke();

        //right arrow
        if (Input.GetKeyDown(KeyCode.RightArrow))
            RightDodge.Invoke();

        //down arrow
        if (Input.GetKeyDown(KeyCode.DownArrow))
            BackDodge.Invoke();

        //up arrow
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Block.Invoke();

        //releasing any of the arrow keys
        bool release = (
            Input.GetKeyUp(KeyCode.LeftArrow) ||
            Input.GetKeyUp(KeyCode.RightArrow) ||
            Input.GetKeyUp(KeyCode.UpArrow) ||
            Input.GetKeyUp(KeyCode.DownArrow));
        if (release)
            Reset.Invoke();

    }
}
