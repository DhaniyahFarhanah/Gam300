using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RougeAI : MonoBehaviour
{
    [Header("AI Logic")]
    public bool debugLog = false;
    public bool Stop = false;  // If true, the car will stop completely
    public bool reverse = true;  // If true, the car will reverse when stuck
    public bool slowWhenAvoiding = true;  // Slow down when avoiding obstacles
    public bool slowWhenTurning = true;  // Slow down during sharp turns
    private GameObject target;
    private enum AIState
    {
        DrivingNormal,
        AvoidingObstacle
    }
    private AIState State = AIState.DrivingNormal;

    [Header("Engine")]
    public float currentSpeed;  // Current speed of the car
    public float maxSpeed = 10f;  // Maximum speed of the car
    [Range(0f, 1f)]
    public float highSpeedThreshold = 0.5f;  // Threshold to define "high speed" (as a percentage of maxSpeed)
    public float maxMotorTorque = 80f;  // Maximum motor torque applied to the wheels
    public float maxBrakeTorque = 150f;  // Maximum brake torque
    public bool isBraking = false;  // Whether the car is currently braking
    public Vector3 centerOfMass;  // Center of mass for the car
    private Rigidbody rb;  // Reference to the car's Rigidbody component

    [Header("Steering")]
    public float maxSteerAngle = 45f;  // Maximum angle the wheels can steer
    public float turnSpeed = 5f;  // Speed at which the car adjusts its steering
    public WheelCollider wheelFL;  // Front left wheel collider
    public WheelCollider wheelFR;  // Front right wheel collider
    public WheelCollider wheelRL;  // Rear left wheel collider
    public WheelCollider wheelRR;  // Rear right wheel collider
    [Range(0f, 1f)]
    public float sharpTurnThreshold = 0.5f;  // Threshold to define a sharp turn
    private float targetSteerAngle = 0f;  // Desired steering angle for the front wheels

    [Header("Sensors")]
    public float sensorLength = 5f;  // Length of the sensors for obstacle detection
    public Vector3 frontSensorPosition;  // Offset for the front sensor's position
    public float frontSideSensorPosition = 0.2f;  // Horizontal offset for the side sensors
    public float frontSensorAngle = 30f;  // Angle for the angled side sensors
    private ObstacleTag sensedObstacle = ObstacleTag.None;  // Whether the car is currently avoiding an obstacle
    private float distanceToObstacle;

    [Header("Slow Detection")]
    [Range(0f, 1f)]
    public float slowSpeedThreshold = 0.5f;  // Speed threshold below which the car is considered "slow"
    public float slowSpeedDuration = 3f;  // Time the car must be slow before triggering reverse
    public float slowTimeCounter = 0f;  // Tracks how long the car has been slow

    [Header("Reversing")]
    public bool isReversing = false;  // Whether the car is currently reversing
    public float reversingDuration = 1f;  // Time the car will spend reversing
    [Range(0f, 1f)]
    public float reverseSpeedMultiplier = 0.5f;  // Speed multiplier for reversing

    [Header("Arrival Logic")]
    public float stoppingDistance = 1f; // Distance to stop at the target
    public float decelerationDistance = 5f; // Distance to start slowing down
    //public bool obstacleDeadAhead;

    private void Start()
    {
        // Get the Rigidbody component and set the center of mass to make the car more stable
        rb = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        target = GameObject.FindGameObjectWithTag("Player");

        // Set Gizmos color to red for debug purposes (when drawing the path in the editor)
        Gizmos.color = Color.red;
    }

    private void FixedUpdate()
    {
        // Call the various functions to control the car's movement
        Sensors();  // Check for obstacles
        Drive();  // Move the car forward
        Braking();  // Handle braking based on the situation
        ApplySteer();  // Steer towards the next waypoint
        LerpToSteerAngle();  // Smoothly adjust the steering angle

        // Only check if the car is stuck when it's not reversing
        if (!isReversing)
            CheckIfSlow();
    }

    // This function handles obstacle detection using raycasting sensors
    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        State = AIState.DrivingNormal;

        // Front right sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetResponse(hit.collider.gameObject);
                avoidMultiplier -= 1f;
            }
        }
        // Front right angled sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, Vector3.up) * transform.forward, out hit, sensorLength * 0.5f))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetResponse(hit.collider.gameObject);
                avoidMultiplier -= 0.5f;
            }
        }
        // Front right 90 angled sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle * 3, Vector3.up) * transform.forward, out hit, sensorLength * 0.25f))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetResponse(hit.collider.gameObject);
                avoidMultiplier -= 0.25f;
            }
        }

        // Front left sensor
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetResponse(hit.collider.gameObject);
                avoidMultiplier += 1f;
            }
        }
        // Front left angled sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, Vector3.up) * transform.forward, out hit, sensorLength * 0.5f))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetResponse(hit.collider.gameObject);
                avoidMultiplier += 0.5f;
            }
        }
        // Front left 90 angled sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle * 3, Vector3.up) * transform.forward, out hit, sensorLength * 0.25f))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            if (hit.collider.gameObject.GetComponent<ObstacleType>())
            {
                SetResponse(hit.collider.gameObject);
                avoidMultiplier += 0.25f;
            }
        }

        // Front center sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                //obstacleDeadAhead = true;

                if (hit.collider.gameObject.GetComponent<ObstacleType>())
                {
                    SetResponse(hit.collider.gameObject);
                    if (State == AIState.AvoidingObstacle)
                    {
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
        distanceToObstacle = hit.distance;

        // Adjust steering based on the obstacle detection
        if (State == AIState.AvoidingObstacle)
        {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }
    }

    // Adjust steering to aim towards the next waypoint
    private void ApplySteer()
    {
        if (State == AIState.AvoidingObstacle)
            return;  // Do not steer towards waypoints if avoiding an obstacle

        Vector3 relativeVector = transform.InverseTransformPoint(target.transform.position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        targetSteerAngle = newSteer;
    }

    // Drive the car forward (or in reverse if necessary)
    private void Drive()
    {
        currentSpeed = rb.velocity.magnitude;  // Get the car's speed from its Rigidbody

        if (currentSpeed < maxSpeed && !isBraking)
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

    // Coroutine to handle reversing the car when it gets stuck
    IEnumerator ReverseCoroutine()
    {
        isReversing = true;  // Set the car to reversing mode
        slowTimeCounter = 0f;  // Reset the slow timer

        yield return new WaitForSeconds(reversingDuration);  // Wait for the reversing duration

        isReversing = false;  // Stop reversing
    }

    // Handle braking based on speed, obstacles, or sharp turns
    private void Braking()
    {
        isBraking = false;

        if (slowWhenAvoiding && State == AIState.AvoidingObstacle && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }
        else if (slowWhenTurning && Mathf.Abs(wheelFL.steerAngle) >= maxSteerAngle * sharpTurnThreshold && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }

        if (isBraking || Stop)
        {
            wheelRL.brakeTorque = maxBrakeTorque;  // Apply brake torque when braking is true
            wheelRR.brakeTorque = maxBrakeTorque;
        }
        else
        {
            wheelRL.brakeTorque = 0;  // Release brakes
            wheelRR.brakeTorque = 0;
        }
    }

    // Smoothly adjust the steering angle
    private void LerpToSteerAngle()
    {
        if (isReversing)
            targetSteerAngle = -targetSteerAngle;  // Reverse steering when reversing

        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    // Check if the car is moving too slowly for too long and trigger reverse if needed
    private void CheckIfSlow()
    {
        if (Stop)
            return;

        if (currentSpeed < slowSpeedThreshold && !isBraking)
        {
            slowTimeCounter += Time.deltaTime;  // Increment slow time counter

            if (slowTimeCounter >= slowSpeedDuration)  // If slow for long enough, trigger reverse
            {
                if (reverse)
                    StartCoroutine(ReverseCoroutine());
                slowTimeCounter = 0f;  // Reset the slow time counter
            }
        }
        else
        {
            slowTimeCounter = 0f;  // Reset the counter if the car is not slow
        }
    }

    private void SetResponse(GameObject obstacle)
    {
        sensedObstacle = obstacle.GetComponent<ObstacleType>().obstacleTag;

        if (sensedObstacle == ObstacleTag.None || sensedObstacle == ObstacleTag.Player)
        {
            State = AIState.DrivingNormal;
        }
        else if (sensedObstacle == ObstacleTag.Light || sensedObstacle == ObstacleTag.Medium || sensedObstacle == ObstacleTag.Heavy || sensedObstacle == ObstacleTag.Pedestrian || sensedObstacle == ObstacleTag.CarAI)
        {
            State = AIState.AvoidingObstacle;
        }
    }
}
