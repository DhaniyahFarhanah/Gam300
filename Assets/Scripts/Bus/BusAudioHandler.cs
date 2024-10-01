using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusAudioHandler : MonoBehaviour
{
    private AudioSource b_AudioSource;
    [SerializeField] public AudioClip[] sCrash;
    [SerializeField] public AudioClip mCrash;
    [SerializeField] public AudioClip lCrash;
    [SerializeField] public AudioClip lose;
    [SerializeField] public AudioClip passengerPop;
    [SerializeField] public AudioClip passengerWee;
    [SerializeField] public AudioClip passengerScream; 
    [SerializeField] public AudioClip passengerDisgust;
    [SerializeField] public AudioClip flush;
    [SerializeField] public AudioClip win;   

    // Start is called before the first frame update
    void Start()
    {
        b_AudioSource = GetComponent<AudioSource>();
    }

    
    public void Play(AudioClip clip)
    {
        if(b_AudioSource.isPlaying)
        {
            return;
        }
        b_AudioSource.clip = clip;
        b_AudioSource.Play();
    }
    public void PlayPriority(AudioClip clip)
    {
        if(b_AudioSource.isPlaying)
        {
            b_AudioSource.Stop();
        }
        b_AudioSource.clip = clip;
        b_AudioSource.Play();
    }

    public void Play(AudioClip[] clips)
    {
        if(b_AudioSource.isPlaying)
        {
            return;
        }
        b_AudioSource.clip = clips[Random.Range(0, clips.Length)];
        b_AudioSource.Play();
    }
}
