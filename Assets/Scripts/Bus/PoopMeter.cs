using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PoopMeter : MonoBehaviour
{
    public float poopMaxTime;
    public float poopCurrentTime;
    public Slider poopSlider;
    private Rigidbody rb;
    private float currentSpeed;

    [Header("UI")]
    public TextMeshProUGUI speedTextUI;
    public TextMeshProUGUI poopTextUI;

    [Header("Penalties")]
    public float minLightSpeed = 10f;
    public float lightObstacle = 1;

    public float minMediumSpeed = 10f;
    public float mediumObstacle = 3;

    public float minHeavySpeed = 50f;
    public float heavyObstacle = 5;

    public float minPedestrianSpeed = 10f;
    public float pedestrian = 10;

    [Header("Wobble Settings")]
    public bool wobbleLight = false;
    public bool wobbleMedium = false;
    public bool wobbleHeavy = false;
    public float wobbleLightDuration = 0.5f;
    public float wobbleLightMagnitude = 0.1f;

    public float wobbleMediumDuration = 0.5f;
    public float wobbleMediumMagnitude = 3f;
    public int wobbleMediumOscillations = 4; // Number of oscillations for medium wobble

    public float wobbleHeavyDuration = 0.5f;
    public float wobbleHeavyMagnitude = 10f;
    public int wobbleHeavyOscillations = 8;  // Number of oscillations for heavy wobble

    private Transform sliderStart;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sliderStart = poopSlider.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (wobbleLight)
        {
            StartCoroutine(WobbleLightEffect());
            wobbleLight = !wobbleLight;
        }
        if (wobbleMedium)
        {
            StartCoroutine(WobbleMediumEffect());
            wobbleMedium = !wobbleMedium;
        }
        if (wobbleHeavy)
        {
            StartCoroutine(WobbleHeavyEffect());
            wobbleHeavy = !wobbleHeavy;
        }

        currentSpeed = rb.velocity.magnitude;
        poopCurrentTime += Time.deltaTime;
        poopSlider.value = poopCurrentTime / poopMaxTime;

        speedTextUI.text = "Speed: " + Mathf.FloorToInt(currentSpeed).ToString();
        poopTextUI.text = "Poop: " + Mathf.FloorToInt(poopCurrentTime) + " / " + Mathf.FloorToInt(poopMaxTime);
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "LightObstacle")
        {
            if (currentSpeed >= minLightSpeed)
                LightCrash();
        }
        else if (collision.gameObject.tag == "MediumObstacle")
        {
            if (currentSpeed >= minMediumSpeed)
                MediumCrash();
            else if (currentSpeed >= minLightSpeed)
                LightCrash();
        }
        else if (collision.gameObject.tag == "HeavyObstacle")
        {
            if (currentSpeed >= minHeavySpeed)
                HeavyCrash();
            else if (currentSpeed >= minMediumSpeed)
                MediumCrash();
            else if (currentSpeed >= minLightSpeed)
                LightCrash();
        }
        else if (collision.gameObject.tag == "Pedestrian")
        {
            if (currentSpeed >= minPedestrianSpeed)
                PedestrianCrash();
        }
    }

    private void LightCrash()
    {
        poopCurrentTime += lightObstacle;
        StartCoroutine(WobbleLightEffect());
    }

    private void MediumCrash()
    {
        poopCurrentTime += mediumObstacle;
        StartCoroutine(WobbleMediumEffect());
    }

    private void HeavyCrash()
    {
        poopCurrentTime += heavyObstacle;
        StartCoroutine(WobbleHeavyEffect());
    }

    private void PedestrianCrash()
    {
        poopCurrentTime += pedestrian;
        StartCoroutine(WobbleHeavyEffect());
    }

    IEnumerator WobbleLightEffect()
    {
        float elapsedTime = 0f;
        Vector3 originalScale = sliderStart.localScale;

        while (elapsedTime < wobbleLightDuration)
        {
            // Calculate wobble scale using a sine wave for smooth oscillation
            float wobbleFactor = 1 + Mathf.Sin(elapsedTime * Mathf.PI * 4) * wobbleLightMagnitude;
            poopSlider.transform.localScale = new Vector3(wobbleFactor, originalScale.y, originalScale.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore the original scale after wobble effect ends
        poopSlider.transform.localScale = originalScale;
    }

    IEnumerator WobbleMediumEffect()
    {
        float elapsedTime = 0f;
        Quaternion originalRotation = Quaternion.identity;

        while (elapsedTime < wobbleMediumDuration)
        {
            // Calculate wobble rotation using a sine wave for smooth oscillation
            float wobbleAngle = Mathf.Sin(elapsedTime * Mathf.PI * wobbleMediumOscillations) * wobbleMediumMagnitude;
            poopSlider.transform.rotation = originalRotation * Quaternion.Euler(0, 0, wobbleAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore the original rotation after wobble effect ends
        poopSlider.transform.rotation = originalRotation;
    }

    IEnumerator WobbleHeavyEffect()
    {
        float elapsedTime = 0f;
        Quaternion originalRotation = Quaternion.identity; // Start with no rotation

        while (elapsedTime < wobbleHeavyDuration)
        {
            // Calculate wobble rotation using a sine wave for smooth side-to-side movement
            float wobbleAngle = Mathf.Sin(elapsedTime * Mathf.PI * wobbleHeavyOscillations) * wobbleHeavyMagnitude;  // Faster oscillation
            poopSlider.transform.rotation = originalRotation * Quaternion.Euler(0, 0, wobbleAngle);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore the original rotation after wobble effect ends
        poopSlider.transform.rotation = originalRotation;
    }
}
