using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PoopMeter : MonoBehaviour
{
    public float poopMaxTime = 180f;
    public float poopCurrentTime;
    public Slider poopSlider;
    private Rigidbody rb;
    private float currentSpeed;

    [Header("UI")]
    public TextMeshProUGUI speedTextUI;
    public TextMeshProUGUI poopTextUI;
    public GameObject loseCanvas;
    public GameObject loseScreen;
    public GameObject winScreen;
    public TextMeshProUGUI disgustTextUI;
    public Image fartScreen;
    public GameObject poopCanvas;
    public Image poopImagePrefab;

    [Header("Penalties")]
    public float minLightSpeed = 10f;
    public float lightObstacle = 1f;

    public float minMediumSpeed = 10f;
    public float mediumObstacle = 3f;

    public float minHeavySpeed = 50f;
    public float heavyObstacle = 5f;

    public float minPedestrianSpeed = 10f;
    public float pedestrian = 10f;

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
    private CollisionHandler collisionHandler;

    private enum State
    {
        Idle,
        LightWobble,
        MediumWobble,
        HeavyWobble
    }
    private State currentState = State.Idle;
    private Transform sliderStart;

    [Header("Air Time")]
    public float minAirTime = 1f;
    public float minAirCrashTime = 2f;
    private float currentAirTime = 0f;
    private ArcadeVehicleController.JeepVisual jeepvisuals;
    private bool inAir = false;
    private Coroutine currentWobbleCoroutine;
    private Coroutine airWobbleCoroutine; // Coroutine for the air wobble

    [Header("Disgust")]
    public bool triggerDisgust = false;
    public int maxDisgust = 4;
    [Range(0f, 1f)]
    public float disgustThreshold = 0.5f;
    [Range(0f, 1f)]
    public float disgustChance = 1f;
    private int currentDisgust = 0;

    [Header("Lose effect")]
    public float maxDisgustEffectDeltaDuration = 1f;
    public float maxDisgustEffectHoldDuration = 3f;
    [Range(0, 255)]
    public float maxDisgustEffectAlpha = 200;
    private bool playingEffect = false;

    [Header("Poop effect")]
    public float poopImageMinScale = 1f;
    public float poopImageMaxScale = 5f;
    public float poopImageGrowlength = 0.5f;
    public float poopImageDelay = 3f;
    public float poopImageVerticalSpeed = 10f;
    public float poopImageHorizontalSpeed = 10f;
    public float poopImageFadeDuration = 10f;
    private List<GameObject> poopImages = new List<GameObject>();
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jeepvisuals = GetComponent<ArcadeVehicleController.JeepVisual>();

        sliderStart = poopSlider.transform;

        loseScreen.SetActive(false);
        maxDisgustEffectAlpha = maxDisgustEffectAlpha / 255f;

        collisionHandler = GetComponent<CollisionHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (wobbleLight)
        {
            LightCrash();
            wobbleLight = !wobbleLight;
        }
        if (wobbleMedium)
        {
            MediumCrash();
            wobbleMedium = !wobbleMedium;
        }
        if (wobbleHeavy)
        {
            HeavyCrash();
            wobbleHeavy = !wobbleHeavy;
        }
        if (triggerDisgust)
        {
            StartCoroutine(MaxDisgustEffect());
            triggerDisgust = !triggerDisgust;
        }

        currentSpeed = rb.velocity.magnitude;

        if (!loseScreen.activeSelf)
        {
            poopCurrentTime += Time.deltaTime;
            poopSlider.value = poopCurrentTime / poopMaxTime;
        }

        if (poopCurrentTime >= poopMaxTime && !loseScreen.activeSelf && !winScreen.activeSelf)
        {
            StartCoroutine(LoseEffect());
        }

        speedTextUI.text = "Speed: " + Mathf.FloorToInt(currentSpeed).ToString();
        poopTextUI.text = "Poop: " + Mathf.FloorToInt(poopCurrentTime) + " / " + Mathf.FloorToInt(poopMaxTime);
        DisgustText();

        // Check if the vehicle is in the air
        if (!jeepvisuals.IsLeftGrounded && !jeepvisuals.IsRightGrounded)
        {
            if (!inAir)
            {
                inAir = true;
                currentAirTime = 0f; // Reset air time when entering the air
            }

            // Accumulate air time while in the air
            currentAirTime += Time.deltaTime;

            // Start air wobble if the vehicle has been in the air longer than minAirTime
            if (currentAirTime >= minAirTime && airWobbleCoroutine == null)
            {
                StartAirWobble(); // Start the air wobble
            }
        }
        else
        {
            if (inAir)
            {
                inAir = false;

                // Stop air wobble if the vehicle has been in the air long enough
                if (currentAirTime >= minAirTime)
                {
                    StopAirWobble(); // Stop the air wobble
                    HeavyCrash(); // Trigger HeavyCrash if applicable
                }

                currentAirTime = 0f; // Reset air time when grounded
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.GetComponent<ObstacleType>())
            return;

        ObstacleTag obstacleType = collision.gameObject.GetComponent<ObstacleType>().obstacleTag;

        if (obstacleType == ObstacleTag.Light)
        {
            if (currentSpeed >= minLightSpeed)
            {
                LightCrash();
            }
        }
        else if (obstacleType == ObstacleTag.Medium)
        {
            if (currentSpeed >= minMediumSpeed)
            {
                MediumCrash();
            }
            else if (currentSpeed >= minLightSpeed)
            {
                LightCrash();
            }
        }
        else if (obstacleType == ObstacleTag.Heavy || obstacleType == ObstacleTag.CarAI)
        {
            if (currentSpeed >= minHeavySpeed)
            {
                HeavyCrash();
            }
            else if (currentSpeed >= minMediumSpeed)
            {
                MediumCrash();
            }
            else if (currentSpeed >= minLightSpeed)
            {
                LightCrash();
            }
        }
        else if (obstacleType == ObstacleTag.Pedestrian)
        {
            if (currentSpeed >= minPedestrianSpeed)
            {
                HeavyCrash();
            }
        }
    }

    private void LightCrash()
    {
        Disgusting();
        poopCurrentTime += lightObstacle;
        StartNewWobble(WobbleLightEffect(), State.LightWobble);
        collisionHandler.ExecuteCollisionShit(ObstacleTag.Light);
        GetComponent<BusAudioHandler>().Play(GetComponent<BusAudioHandler>().sCrash);
    }

    private void MediumCrash()
    {
        Disgusting();
        poopCurrentTime += mediumObstacle;
        StartNewWobble(WobbleMediumEffect(), State.MediumWobble);
        collisionHandler.ExecuteCollisionShit(ObstacleTag.Medium);
        GetComponent<BusAudioHandler>().Play(GetComponent<BusAudioHandler>().mCrash);
    }

    private void HeavyCrash()
    {
        Disgusting();
        poopCurrentTime += heavyObstacle;
        StartNewWobble(WobbleHeavyEffect(), State.HeavyWobble);
        collisionHandler.ExecuteCollisionShit(ObstacleTag.Heavy);
        GetComponent<BusAudioHandler>().Play(GetComponent<BusAudioHandler>().lCrash);
    }

    private void PedestrianCrash()
    {
        Disgusting();
        poopCurrentTime += pedestrian;
        StartNewWobble(WobbleHeavyEffect(), State.HeavyWobble);
        collisionHandler.ExecuteCollisionShit(ObstacleTag.Pedestrian);
    }

    // Method to start a new wobble, stopping the previous one if needed
    private void StartNewWobble(IEnumerator wobbleEffect, State newState)
    {
        switch (newState)
        {
            case State.LightWobble:
                if (currentState != State.Idle)
                {
                    return;
                }
                break;
            case State.MediumWobble:
                if (!(currentState == State.Idle || currentState == State.LightWobble))
                {
                    return;
                }
                break;
            case State.HeavyWobble:
                if (!(currentState == State.Idle || currentState == State.LightWobble || currentState == State.MediumWobble))
                {
                    return;
                }
                break;
        }

        // Stop the previous wobble coroutine if it's running
        if (currentWobbleCoroutine != null)
        {
            StopCoroutine(currentWobbleCoroutine);
        }

        // Set the new state
        currentState = newState;

        // Start the new wobble coroutine
        currentWobbleCoroutine = StartCoroutine(wobbleEffect);
    }

    IEnumerator WobbleLightEffect()
    {
        float elapsedTime = 0f;
        Vector3 originalScale = sliderStart.localScale;

        while (elapsedTime < wobbleLightDuration)
        {
            // Calculate wobble scale using a sine wave for smooth oscillation
            float wobbleFactor = 1 + Mathf.Sin(elapsedTime * Mathf.PI * 4) * wobbleLightMagnitude;
            poopSlider.transform.localScale = new Vector3(wobbleFactor * originalScale.x, originalScale.y, originalScale.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore the original scale after wobble effect ends
        poopSlider.transform.localScale = originalScale;
        currentState = State.Idle;
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
        currentState = State.Idle;
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
        currentState = State.Idle;
    }

    IEnumerator LoseEffect()
    {
        loseCanvas.SetActive(true);
        loseScreen.SetActive(true);
        loseScreen.transform.localScale = Vector3.zero; // Start from zero scale

        float duration = 0.5f; // Duration of the effect
        float elapsedTime = 0f;
        Vector3 finalScale = Vector3.one; // Final scale (1, 1, 1)

        GetComponent<BusAudioHandler>().PlayPriority(GetComponent<BusAudioHandler>().lose);
        // Loop to create the linear scaling effect
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // Linear scale-up: scaling directly based on progress (from 0 to 1)
            loseScreen.transform.localScale = new Vector3(progress, progress, progress);

            yield return null;
        }

        // Ensure final scale is exactly 1 when finished
        loseScreen.transform.localScale = finalScale;

    }

    private void StartAirWobble()
    {
        // Start the air wobble if it isn't already running
        if (airWobbleCoroutine == null)
        {
            airWobbleCoroutine = StartCoroutine(ContinuousMediumWobble());
        }
    }

    private void StopAirWobble()
    {
        // Stop the air wobble if it's running
        if (airWobbleCoroutine != null)
        {
            StopCoroutine(airWobbleCoroutine);
            airWobbleCoroutine = null;
        }
    }

    // Continuous medium wobble while in the air
    IEnumerator ContinuousMediumWobble()
    {
        while (inAir)
        {
            yield return WobbleMediumEffect(); // Continuously run the medium wobble effect
        }
    }

    private void Disgusting()
    {
        if (poopCurrentTime >= poopMaxTime * disgustThreshold)
        {
            if (Random.Range(0f, 1f) <= disgustChance)
            {
                if (currentDisgust < maxDisgust)
                {
                    if (GetComponent<ArcadeVehicleController.Vehicle>().m_Passengers > 0)
                    {
                        ++currentDisgust;
                    }
                    CreateOnScreenPoop();
                }
                if (currentDisgust >= maxDisgust)
                {
                    StartCoroutine(MaxDisgustEffect());
                }
            }
        }
    }

    IEnumerator MaxDisgustEffect()
    {
        if (!playingEffect)
        {
            playingEffect = true;
            float elapsedTime = 0f;
            Color fartScreenColor = fartScreen.color;
            GetComponent<ArcadeVehicleController.Vehicle>().ThrowPassengers(false);
            BusStopPickUp[] allStops = FindObjectsOfType<BusStopPickUp>();
            foreach (BusStopPickUp stop in allStops)
            {
                stop.Reset();
            }
            ClearPoopImages();

            GetComponent<BusAudioHandler>().Play(GetComponent<BusAudioHandler>().passengerDisgust);
            // Fade in (increase alpha)
            while (elapsedTime < maxDisgustEffectDeltaDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / maxDisgustEffectDeltaDuration;
                fartScreenColor.a = Mathf.Lerp(0, maxDisgustEffectAlpha, progress);
                fartScreen.color = fartScreenColor;
                yield return null;
            }

            // Hold the full alpha for a short time
            yield return new WaitForSeconds(maxDisgustEffectHoldDuration);

            currentDisgust = 0;

            // Fade out (decrease alpha)
            elapsedTime = 0f; // Reset elapsed time for fading out
            while (elapsedTime < maxDisgustEffectDeltaDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / maxDisgustEffectDeltaDuration;
                fartScreenColor.a = Mathf.Lerp(maxDisgustEffectAlpha, 0, progress);
                fartScreen.color = fartScreenColor;
                yield return null;
            }

            // Ensure alpha is completely set to 0 at the end
            fartScreenColor.a = 0;
            fartScreen.color = fartScreenColor;
            playingEffect = false;
        }
    }

    private void DisgustText()
    {
        if (poopCurrentTime <= poopMaxTime * disgustThreshold ||
                GetComponent<ArcadeVehicleController.Vehicle>().m_Passengers <= 0)
        {
            disgustTextUI.text = "";
        }
        else
        {
            if (currentDisgust > maxDisgust * 3 / 4f)
            {
                disgustTextUI.text = "Passengers Mood: Abhorred";
            }
            else if (currentDisgust > maxDisgust * 2 / 4f)
            {
                disgustTextUI.text = "Passengers Mood: Repulsed";
            }
            else if (currentDisgust > maxDisgust * 1 / 4f)
            {
                disgustTextUI.text = "Passengers Mood: Annoyed";
            }
            else
            {
                disgustTextUI.text = "Passengers Mood: Pleasant";
            }
        }
    }

    private void CreateOnScreenPoop()
    {
        // Instantiate the poop image (ensure poopImagePrefab is a GameObject or use .gameObject if it's an Image)
        GameObject poopImage = Instantiate(poopImagePrefab.gameObject, poopCanvas.transform);

        // Get the RectTransform of the poop image
        RectTransform rectTransform = poopImage.GetComponent<RectTransform>();

        // Randomize position within the canvas bounds
        RectTransform canvasRect = poopCanvas.transform.parent.GetComponent<RectTransform>();

        float randomX = Random.Range(canvasRect.rect.width * -0.5f, canvasRect.rect.width * 0.5f);
        float randomY = Random.Range(canvasRect.rect.height * -0.5f, canvasRect.rect.height * 0.5f);
        rectTransform.anchoredPosition = new Vector2(randomX, randomY);

        // Randomize rotation (for z-axis, since it's a 2D object)
        float randomRotation = Random.Range(0f, 360f);
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, randomRotation);

        // Randomize size (scale)
        float randomScale = Random.Range(poopImageMinScale, poopImageMaxScale); // Adjust range as needed
        Vector3 finalScale = new Vector3(randomScale, randomScale, 1f);

        // Initially, set the scale to zero for expansion effect
        rectTransform.localScale = Vector3.zero;

        // Add poop image to the list
        poopImages.Add(poopImage);

        // Start the expansion effect coroutine
        StartCoroutine(ExpandPoopImage(poopImage, finalScale));
        StartCoroutine(MovePoopDownAndRemove(poopImage, rectTransform));
    }

    private IEnumerator ExpandPoopImage(GameObject poopImage, Vector3 finalScale)
    {
        if (poopImage == null) yield break;  // Exit the coroutine if the object is already destroyed

        RectTransform rectTransform = poopImage.GetComponent<RectTransform>();

        float elapsedTime = 0f;

        // Loop to create the linear scaling effect
        while (elapsedTime < poopImageGrowlength)
        {
            // Check if poopImage or rectTransform is null (destroyed) during the loop
            if (poopImage == null || rectTransform == null) yield break;

            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / poopImageGrowlength;

            // Linear scale-up: scaling from 0 to final size
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, finalScale, progress);

            yield return null;
        }

        // Ensure final scale is set exactly at the end
        if (rectTransform != null)
        {
            rectTransform.localScale = finalScale;
        }
    }
    private IEnumerator MovePoopDownAndRemove(GameObject poopImage, RectTransform rectTransform)
    {
        float exitScreenYPosition = -poopCanvas.transform.parent.GetComponent<RectTransform>().rect.height / 2 - rectTransform.rect.height;  // Y position off the screen
        float elapsedTime = 0f;

        // Wait for the delay before starting to move
        yield return new WaitForSeconds(poopImageDelay);

        // Total time for moving and fading
        float fadeStartTime = elapsedTime;  // Start fading as soon as the movement begins
        float fadeEndTime = poopImageFadeDuration;

        Image poopImageComponent = poopImage.GetComponent<Image>();

        // Move the poop image down the screen with horizontal movement only when rotating
        while (rectTransform != null && rectTransform.anchoredPosition.y > exitScreenYPosition && poopImageComponent.color.a > 0f)
        {
            // Get the angular velocity of the vehicle's rigidbody (rotation around y-axis)
            float angularVelocityY = rb.angularVelocity.y;

            // If the vehicle is rotating, move the poop image to the left or right
            if (Mathf.Abs(angularVelocityY) > 0.1f) // A small threshold to detect rotation
            {
                // Calculate horizontal movement speed based on the angular velocity magnitude
                float horizontalDirection = angularVelocityY > 0 ? 1f : -1f;
                float adjustedHorizontalSpeed = Mathf.Abs(angularVelocityY) * poopImageHorizontalSpeed;

                // Move the poop image based on the vehicle's rotation and adjusted speed
                rectTransform.anchoredPosition += new Vector2(horizontalDirection * adjustedHorizontalSpeed * Time.deltaTime, 0f);
            }

            // Move the poop image down
            rectTransform.anchoredPosition += new Vector2(0f, -poopImageVerticalSpeed * Time.deltaTime);

            // Start fading out the image
            if (elapsedTime >= fadeStartTime)
            {
                float fadeProgress = Mathf.Clamp01((elapsedTime - fadeStartTime) / poopImageFadeDuration);

                // Adjust alpha of the image
                Color tempColor = poopImageComponent.color;
                tempColor.a = Mathf.Lerp(1.0f, 0.0f, fadeProgress);
                poopImageComponent.color = tempColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // Remove the poop image from the list and destroy the GameObject once it's off-screen
        if (poopImage != null)
        {
            poopImages.Remove(poopImage);
            Destroy(poopImage);
        }
    }


    public void ReducePoop(float time)
    {
        poopCurrentTime = poopCurrentTime - time < 0f ? 0f : poopCurrentTime - time;
        ClearPoopImages();
    }

    public void ResetDisgust()
    {
        currentDisgust = 0;
    }

    public void ClearPoopImages()
    {
        foreach (GameObject image in poopImages)
        {
            Destroy(image);
        }
        poopImages.Clear();
    }

    public float GetCurrentSpeed() {
        return currentSpeed;
    }
}
