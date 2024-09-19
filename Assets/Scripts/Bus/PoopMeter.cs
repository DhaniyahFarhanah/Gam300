using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoopMeter : MonoBehaviour
{
    public float poopMaxTime;
    public float poopCurrentTime;
    public Slider poopSlider;

    [Header("Penalties")]
    public float lightObstacle = 1;
    public float mediumObstacle = 3;
    public float HeavyObstacle = 5;
    public float Pedestrian = 10;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "LightObstacle")
        {
            poopCurrentTime += lightObstacle;
        }
        else if (collision.gameObject.tag == "MediumObstacle")
        {
            poopCurrentTime += mediumObstacle;
        }
        else if (collision.gameObject.tag == "HeavyObstacle")
        {
            poopCurrentTime += HeavyObstacle;
        }
        else if (collision.gameObject.tag == "Pedestrian")
        {
            poopCurrentTime += Pedestrian;
        }
    }
}
