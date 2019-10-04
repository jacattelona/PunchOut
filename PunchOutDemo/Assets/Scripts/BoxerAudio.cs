using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerAudio : MonoBehaviour
{
    AudioSource source;

    public AudioClip[] punches;
    public AudioClip[] dodges;
    public AudioClip[] hits;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        
    }


    public void PlayPunch()
    {
        int choice = (int) Random.Range(0, punches.Length);
        source.PlayOneShot(punches[choice]);
    }

    public void PlayDodge()
    {
        int choice = (int)Random.Range(0, punches.Length);
        source.PlayOneShot(dodges[choice]);
    }

    public void PlayHit()
    {
        int choice = (int)Random.Range(0, punches.Length);
        source.PlayOneShot(hits[choice]);
    }
}
