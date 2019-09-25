using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public Transform cameraTransform;
    private Vector3 cameraVelocity = Vector3.zero;

    [SerializeField]
    public GameObject mainMenu, offensiveTraining, defensiveTraining, match;

    private const int STATE_MAIN_MENU = 0, STATE_OFFENSIVE = 1, STATE_DEFENSIVE = 3, STATE_MATCH = 4;

    private int state = STATE_MAIN_MENU;



    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Move between the phases
        switch (state)
        {
            case STATE_MAIN_MENU:
                break;
            case STATE_OFFENSIVE:
                break;
            case STATE_DEFENSIVE:
                break;
            case STATE_MATCH:
                break;
        }
    }
    
    private bool MoveCameraToPosition(Vector3 position)
    {
        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, position, ref cameraVelocity, .5f);
        return Vector3.Distance(cameraTransform.position, position) < .01f;
    }

}
