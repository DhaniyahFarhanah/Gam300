using ArcadeVehicleController;
using System.Collections;
using UnityEngine;

public class OverturnChecker : MonoBehaviour
{
    [SerializeField] private float overturnAngleThreshold = 60f; // More lenient angle check for overturns
    [SerializeField] private float minSpeedThreshold = 0.1f; // Slightly higher speed threshold
    [SerializeField] private float resetHeight = 0.2f; // Adjustable reset height
    [SerializeField] private float autoFlipDelay = 3f; // Time the bus must be overturned before auto-flip
    [SerializeField] private float manualFlipCooldown = 5f; // Cooldown for manually flipping the bus

    [SerializeField] private Vehicle m_Bus;
    [SerializeField] private GameObject m_BusVisual;
    [SerializeField] private GameObject m_CloudSpawn; // Cloud spawn for visual effects

    private Rigidbody m_BusRigidbody;
    private bool m_Overturned;
    private bool m_IsFlipping; // To prevent multiple flips happening
    private float lastFlipTime = 0f; // Cooldown timer for flips
    private float overturnedStartTime = 0f; // Time when the bus was first overturned
    private bool isManuallyFlipped = false; // Flag to indicate manual flip cooldown

    void Start()
    {
        m_Bus = GetComponent<Vehicle>();
        m_BusRigidbody = m_Bus.GetComponent<Rigidbody>(); // Get the bus's Rigidbody component
    }

    void Update()
    {
        Vector3 eulers = this.transform.rotation.eulerAngles;

        // Check if the bus is overturned based on both X and Z axes
        bool isOverturnedX = Mathf.Abs(NormalizeAngle(eulers.x)) > overturnAngleThreshold;
        bool isOverturnedZ = Mathf.Abs(NormalizeAngle(eulers.z)) > overturnAngleThreshold;

        // Check if the bus is almost stationary
        bool isBusStationary = m_BusRigidbody.velocity.magnitude < minSpeedThreshold;

        // Manual reset of the bus using the R key with a cooldown, only if the bus is overturned
        if (Input.GetKeyUp(KeyCode.R) && (isOverturnedX || isOverturnedZ) && Time.time - lastFlipTime > manualFlipCooldown && !isManuallyFlipped)
        {
            StartCoroutine(FlipBusBack());
            isManuallyFlipped = true; // Set flag for manual flip cooldown
        }

        // Automatically detect overturn when stationary and angles exceed the threshold
        else if ((isOverturnedX || isOverturnedZ) && isBusStationary && !m_IsFlipping)
        {
            if (overturnedStartTime == 0f)
            {
                overturnedStartTime = Time.time; // Record the time when the bus first overturned
            }

            // Check if the bus has been overturned for the required delay
            if (Time.time - overturnedStartTime >= autoFlipDelay)
            {
                m_Overturned = true;
            }
        }
        else
        {
            overturnedStartTime = 0f; // Reset the overturn start time if not overturned anymore
        }

        // Perform the automatic flipback if overturned
        if (m_Overturned)
        {
            StartCoroutine(FlipBusBack());
            m_Overturned = false;
        }

        // Reset the manual flip cooldown after the cooldown time has passed
        if (isManuallyFlipped && Time.time - lastFlipTime > manualFlipCooldown)
        {
            isManuallyFlipped = false;
        }
    }

    IEnumerator FlipBusBack()
    {
        m_IsFlipping = true; // Prevent multiple flips at once
        lastFlipTime = Time.time; // Set cooldown time

        // Pause the physics by setting isKinematic to true
        m_BusRigidbody.isKinematic = true;

        Vector3 busRotation = transform.rotation.eulerAngles;
        Vector3 busPosition = transform.position;
        m_BusVisual.SetActive(false);

        // Optional: Trigger cloud spawn effect here if needed
        if (m_CloudSpawn != null)
        {
            m_CloudSpawn.SetActive(true);
        }

        yield return new WaitForSeconds(0.1f);

        // Disable the bus's control and adjust its position and rotation
        m_Bus.enabled = false;
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, busRotation.y, 0.0f));
        transform.position = new Vector3(busPosition.x, busPosition.y + resetHeight, busPosition.z);

        yield return new WaitForSeconds(0.2f);

        // Re-enable the bus and physics after the flip
        m_Bus.enabled = true;
        m_BusVisual.SetActive(true);

        // Re-enable physics
        m_BusRigidbody.isKinematic = false;

        // Optional: Deactivate cloud spawn effect after flip
        if (m_CloudSpawn != null)
        {
            m_CloudSpawn.SetActive(false);
        }

        m_IsFlipping = false; // Reset flip state after completion
    }

    // Normalize the angle to the range [-180, 180] for easier comparisons
    private float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}
