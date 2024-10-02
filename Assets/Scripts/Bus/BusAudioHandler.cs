using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BusAudioHandler : MonoBehaviour
{
    private AudioSource b_AudioSource;
    private AudioSource bgm_AudioSource1;
    private AudioSource bgm_AudioSource2;
    [SerializeField] float volumebgm = 0.6f;
    [SerializeField] float fadeTime = 10.0f;
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
    [SerializeField] public AudioClip MenuSoundtrack;
    [SerializeField] public AudioClip WinningSoundtrack;
    [SerializeField] public AudioClip PoliceAlert;

    private bool isFading = false;
    private bool useSource1 = true;
    
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
        bgm_AudioSource2.volume = 0.0f;
        bgm_AudioSource2.clip = DrivingSoundtrack[1];
    }

    void Update() {
        if(!isFading) {
            if(useSource1 && !bgm_AudioSource1.isPlaying) {
                StartCoroutine(FadeTracks(bgm_AudioSource2, bgm_AudioSource1, DrivingSoundtrack[1]));
            }
            else if(!useSource1 && !bgm_AudioSource2.isPlaying) {
                StartCoroutine(FadeTracks(bgm_AudioSource1, bgm_AudioSource2, DrivingSoundtrack[0]));
            }
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

    private IEnumerator FadeTracks(AudioSource to, AudioSource from, AudioClip next) 
    {
        isFading = true;
        float timer = 0f;

        to.clip = next;
        to.Play();

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer/fadeTime;

            from.volume = Mathf.Lerp(volumebgm, 0.0f, t);
            to.volume = Mathf.Lerp(0.0f, volumebgm, t);

            yield return null;
        }

        from.volume = 0f;
        to.volume = volumebgm;
        from.Stop();

        useSource1 = !useSource1;
        isFading = false;
    }
}
