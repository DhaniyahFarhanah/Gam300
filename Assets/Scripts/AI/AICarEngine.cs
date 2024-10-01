using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarEngine : MonoBehaviour
{
    [Header("Debug")]
    public bool logEnabled = false;
    public bool stop = false;  
    public bool reversible = true;
    public bool debugLine = false;
    public Color sensorLineColor;
    public Color targetLineColor;
    protected bool debugOnce = false;

    [Header("Engine")]
    public float maxMotorTorque = 25f;
    public float maxBrakeTorque = 500f;
    public float currentSpeed;  
    public float maxSpeed = 8f;  
    [Range(0f, 1f)]
    public float highSpeedThreshold = 0.5f;  
    public bool isBraking = false;
    public Vector3 centerOfMass;
    private Rigidbody rb;

    [Header("Steering")]
    public float maxSteerAngle = 45f; 
    public float turnSpeed = 5f;      
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    [Range(0f, 1f)]
    public float sharpTurnThreshold = 0.5f;
    protected Vector3 targetPosition;
    public float targetSteerAngle = 0f;
    protected float avoidMultiplier = 0f;

    [Header("Sensors")]
    public float sensorLength = 10f;                
    public Vector3 frontSensorPosition;   
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;
    public bool detectedObstacle;
    protected RaycastHit detectedObstacleHit;

    [Header("Slow Detection")]
    [Range(0f, 1f)]
    public float slowSpeedThreshold = 0.10f;  // Speed threshold below which the car is considered "slow"
    public float slowSpeedDuration = 3f;  // Time the car must be slow before triggering reverse
    public float slowTimeCounter = 0f;  // Tracks how long the car has been slow

    [Header("Reversing")]
    public bool isReversing = false;  // Whether the car is currently reversing
    public float reversingDuration = 1f;  // Time the car will spend reversing
    [Range(0f, 1f)]
    public float reverseSpeedMultiplier = 0.5f;  // Speed multiplier for reversing

    [Header("Visuals")]
    [SerializeField] private bool brakeVisual = true;
    [SerializeField] private Material brakeMaterial;
    [SerializeField] private Material neutralMaterial;
    [SerializeField] private Renderer brakeLightLeft;
    [SerializeField] private Renderer brakeLightRight;


    protected void Init()
    {
        rb = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
    }

    protected void EngineUpdate()
    {
        // Call the various functions to control the car's movement
        Sensors();  // Check for obstacles
        ObstacleResponse();
        BrakeLogic();
        Braking();  // Handle braking based on the situation
        ApplySteer();  // Steer towards the next waypoint
        LerpToSteerAngle();  // Smoothly adjust the steering angle
        Drive();  // Move the car forward

        // Only check if the car is stuck when it's not reversing
        if (!isReversing)
            CheckIfSlow();
    }

    #region Sensor
    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        avoidMultiplier = 0;
        detectedObstacle = false;

        // Front right sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            DrawDebugLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetDetectedObstacle(hit);
                avoidMultiplier -= 1f;
            }
        }
        // Front right angled sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, Vector3.up) * transform.forward, out hit, sensorLength * 0.5f))
        {
            DrawDebugLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetDetectedObstacle(hit);
                avoidMultiplier -= 0.5f;
            }
        }
        // Front right 90 angled sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle * 3, Vector3.up) * transform.forward, out hit, sensorLength * 0.25f))
        {
            DrawDebugLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetDetectedObstacle(hit);
                avoidMultiplier -= 0.25f;
            }
        }

        // Front left sensor
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            DrawDebugLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetDetectedObstacle(hit);
                avoidMultiplier += 1f;
            }
        }
        // Front left angled sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, Vector3.up) * transform.forward, out hit, sensorLength * 0.5f))
        {
            DrawDebugLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetDetectedObstacle(hit);
                avoidMultiplier += 0.5f;
            }
        }
        // Front left 90 angled sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle * 3, Vector3.up) * transform.forward, out hit, sensorLength * 0.25f))
        {
            DrawDebugLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetDetectedObstacle(hit);
                avoidMultiplier += 0.25f;
            }
        }

        // Front center sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
            {
                DrawDebugLine(sensorStartPos, hit.point);
                if (hit.collider.gameObject.GetComponent<ObstacleType>())
                {
                    SetDetectedObstacle(hit);
                    if (hit.normal.x < 0)
                    {
                        avoidMultiplier -= 1;
                    }
                    else
                    {
                        avoidMultiplier += 1;
                    }
                }
            }
        }
    }

    private void SetDetectedObstacle(RaycastHit detectedObject)
    {
        detectedObstacle = true;
        detectedObstacleHit = detectedObject;
    }

    protected virtual void ObstacleResponse()
    {    }

    private void DrawDebugLine(Vector3 start, Vector3 end)
    {
        if (debugLine)
            Debug.DrawLine(start, end, sensorLineColor); // Pass the color to Debug.DrawLine
    }
    #endregion Sensor

    #region Braking
    protected virtual void BrakeLogic()
    {

    }

    private void Braking()
    { 
        if (isBraking || stop)
        {
            wheelRL.brakeTorque = maxBrakeTorque;  // Apply brake torque when braking is true
            wheelRR.brakeTorque = maxBrakeTorque;

            if (brakeVisual && brakeLightLeft.material != brakeMaterial)
            {
                brakeLightLeft.material = brakeMaterial;
                brakeLightRight.material = brakeMaterial;
            }
        }
        else
        {
            wheelRL.brakeTorque = 0;  // Release brakes
            wheelRR.brakeTorque = 0;

            if (brakeVisual && brakeLightLeft.material != neutralMaterial)
            {
                brakeLightLeft.material = neutralMaterial;
                brakeLightRight.material = neutralMaterial;
            }
        }
    }
    #endregion Braking

    #region Steering
    protected virtual void ApplySteer()
    {
        if (debugLine)
            Debug.DrawLine(transform.position, targetPosition, targetLineColor);

        Vector3 relativeVector = transform.InverseTransformPoint(targetPosition);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        targetSteerAngle = newSteer;
    }

    private void LerpToSteerAngle()
    {
        if (isReversing)
            targetSteerAngle = -targetSteerAngle;  // Reverse steering when reversing

        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }
    #endregion Steering

    #region Driving
    private void Drive()
    {
        currentSpeed = rb.velocity.magnitude;  // Get the car's speed from its Rigidbody

        if (currentSpeed < maxSpeed && !isBraking && !stop)
        {
            if (!isReversing)
            {
                wheelFL.motorTorque = maxMotorTorque;  // Apply forward torque to the wheels
                wheelFR.motorTorque = maxMotorTorque;
            }
            else
            {
                wheelFL.motorTorque = -maxMotorTorque * reverseSpeedMultiplier;  // Apply reverse torque
                wheelFR.motorTorque = -maxMotorTorque * reverseSpeedMultiplier;
            }
        }
        else
        {
            wheelFL.motorTorque = 0;  // Stop applying motor torque if at max speed
            wheelFR.motorTorque = 0;
        }
    }
    #endregion Driving

    #region Reverse
    private void CheckIfSlow()
    {
        if (stop)
            return;

        if (currentSpeed < maxSpeed * slowSpeedThreshold && !isBraking)
        {
            slowTimeCounter += Time.deltaTime;  // Increment slow time counter

            if (slowTimeCounter >= slowSpeedDuration)  // If slow for long enough, trigger reverse
            {
                if (reversible)
                    StartCoroutine(ReverseCoroutine());
                slowTimeCounter = 0f;  // Reset the slow time counter
            }
        }
        else
        {
            slowTimeCounter = 0f;  // Reset the counter if the car is not slow
        }
    }

    IEnumerator ReverseCoroutine()
    {
        isReversing = true;  // Set the car to reversing mode
        slowTimeCounter = 0f;  // Reset the slow timer

        yield return new WaitForSeconds(reversingDuration);  // Wait for the reversing duration

        isReversing = false;  // Stop reversing
    }
    #endregion Reverse
}
