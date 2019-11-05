using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardIndicator : MonoBehaviour
{
    public ImitationSystem visibleAI, invisibleAI;
    float rewardsActive = 0;
    [SerializeField]
    float decayRate = 3f;
    [SerializeField]
    float addRate = .25f;
    Renderer rend;

    bool active = false;
    int activeCounter = 0;

    Transform parent;
    float sizeMax = .5f;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        parent = transform.parent;
    }

    // Start is called before the first frame update
    void Start()
    {
        visibleAI.rewardEvent.AddListener(TrackReward);
        invisibleAI.rewardEvent.AddListener(TrackReward);
        rend = GetComponent<MeshRenderer>();
        rend.material.SetColor("_EmissionColor", new Color(.8f, .8f, 0.0f, 1.0f) * rewardsActive);
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            return;
        }


        if (rewardsActive >= 0)
            rewardsActive -= Time.deltaTime*decayRate;
        
        if (rewardsActive >= sizeMax) rewardsActive = sizeMax;

        float scale = rewardsActive + .1f;

        parent.localScale = new Vector3(1.0f, scale, 1.0f);
        //transform.localScale = new Vector3(scale, scale, scale);
        //rend.material.SetColor("_EmissionColor", new Color(1.0f, 1.0f, 1.0f, 1.0f) * rewardsActive);
    }

    void FixedUpdate()
    {
        //activeCounter++;
        //if (activeCounter > 100) active = true;
        if (sizeMax < 3.0f)
        {
            sizeMax += Time.deltaTime * .05f;
        }
    }

    void TrackReward()
    {
        if (active) rewardsActive += addRate;
    }

    public void Activate()
    {
        active = true;
    }
}
