using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BusAudioHandler : MonoBehaviour
{
    private AudioSource b_AudioSource;
    private AudioSource bgm_AudioSource1;
    private AudioSource bgm_AudioSource2;
    [SerializeField] float volumebgm = 1.0f;
    //[SerializeField] float fadeTime = 2.0f;
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
    [SerializeField] public AudioClip[] DrivingSoundtrack;
    //[SerializeField] public AudioClip MenuSoundtrack;
    [SerializeField] public AudioClip WinningSoundtrack;
    [SerializeField] public AudioClip PoliceAlert;

    //private bool isFading = false;
    private bool ActiveAudioSource = true;
    
    // Start is called before the first frame update
    void Start()
    {
        b_AudioSource = GetComponent<AudioSource>();
        
        
        GameObject PlayerCamera = GameObject.Find("Player Camera");
        bgm_AudioSource1 = PlayerCamera.GetComponent<AudioSource>();
        bgm_AudioSource2 = PlayerCamera.AddComponent<AudioSource>();
        bgm_AudioSource1.clip = DrivingSoundtrack[0];
        bgm_AudioSource1.volume = volumebgm;
        bgm_AudioSource1.Play();
        bgm_AudioSource2.volume = volumebgm;
        bgm_AudioSource2.clip = DrivingSoundtrack[1];
        
    }

    void Update() {
        if(ActiveAudioSource && !bgm_AudioSource1.isPlaying) {
            ActiveAudioSource  = false;
            bgm_AudioSource2.loop = true;
            bgm_AudioSource2.Play();
        }

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

    

    public void PauseGameBGM() {
        bgm_AudioSource2.Pause();
    }

    public void ResumeGameBGM() {
        bgm_AudioSource2.Play();
    }
}
