using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoopMeter : MonoBehaviour
{
    public float poopMaxTime;
    public float poopCurrentTime;
    public Slider poopSlider;

    // Start is called before the first frame update
    void Start()
    {   
    }

    // Update is called once per frame
    void Update()
    {
        poopCurrentTime += Time.deltaTime;
        poopSlider.value = poopCurrentTime / poopMaxTime;
    }
}
