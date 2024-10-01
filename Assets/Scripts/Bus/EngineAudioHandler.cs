using System.Collections;
using System.Collections.Generic;
using ArcadeVehicleController;
using UnityEngine;

public class EngineAudioHandler : MonoBehaviour
{
    private AudioSource _AudioSource;
    public AudioClip engineLoop;
    public float maxSpeed = 80.0f;
    public float minPitch = 0.1f;
    public float maxPitch = 1.0f;
    private GameObject _bus;

    // Start is called before the first frame update
    void Start()
    {
        _bus = GameObject.FindGameObjectWithTag("Player");
        _AudioSource = GetComponent<AudioSource>();
        _AudioSource.volume = 0.5f;
        _AudioSource.clip = engineLoop;
        _AudioSource.loop = true;
        _AudioSource.Play();
    }
    
    void Update()
    {
        PoopMeter _poopMeter = _bus.GetComponent<PoopMeter>();
        if(_poopMeter.GetCurrentSpeed() > 0.0f)
        {
            AdjustPitch();
        }

    }

    void AdjustPitch()
    {
        //float maxSpeed = _bus.GetComponent<Vehicle>().MaxSpeed;        
        float pitch = Mathf.Lerp(minPitch, maxPitch, _bus.GetComponent<PoopMeter>().GetCurrentSpeed()/maxSpeed);

        _AudioSource.pitch = pitch;
    }
    
}
