using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarEngine : MonoBehaviour
{
    public Transform path;

    [Header("Engine")]
    public float currentSpeed;
    public float maxSpeed = 100f;
    public float maxMotorTorque = 80f;
    public float maxBrakeTorque = 150f;
    public bool isBraking = false;
    public Vector3 centerOfMass;

    [Header("Steering")]
    public float maxSteerAngle = 45f;
    public float turnSpeed = 5f;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;

    [Header("Sensors")]
    public float sensorLength = 5f;
    public Vector3 frontSensorPosition;
    public float frontSideSensorPosition = 0.2f;
    public float frontSensorAngle = 30f;

    [Header("Slow Detection")]
    public float slowSpeedThreshold = 3f; // The speed threshold below which the car is considered "slow"
    public float slowSpeedDuration = 3f; // Time the car must be slow to trigger reverse
    public float slowTimeCounter = 0f;    // Tracks how long the car has been slow

    [Header("Reversing")]
    public bool isReversing = false;
    public float reversingDuration = 1f;

    private List<Transform> nodes = new List<Transform>();
    private int currentNode = 0;
    private bool avoiding = false;
    private float targetSteerAngle = 0f;

    private void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;

        Transform[] pathTransform = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        for (int i = 0; i < pathTransform.Length; ++i)
        {
            if (pathTransform[i] != path.transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }

        Gizmos.color = Color.red;
    }

    private void FixedUpdate()
    {
        Sensors();
        ApplySteer();
        Drive();
        CheckWaypointDistance();
        Braking();
        LerpToSteerAngle();

        if (!isReversing)
            CheckIfSlow();
    }

    private void Sensors()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * frontSensorPosition.z;
        sensorStartPos += transform.up * frontSensorPosition.y;
        float avoidMultiplier = 0;
        avoiding = false;

        //Front right sensor
        sensorStartPos += transform.right * frontSideSensorPosition;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier -= 1f;
        }
        //Front right angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle, Vector3.up) * transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier -= 0.5f;
        }
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(frontSensorAngle * 3, Vector3.up) * transform.forward, out hit, sensorLength / 2))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier -= 0.25f;
        }

        //Front left sensor
        sensorStartPos -= transform.right * frontSideSensorPosition * 2;
        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier += 1f;
        }
        //Front left angle sensor
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle, Vector3.up) * transform.forward, out hit, sensorLength))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier += 0.5f;
        }
        else if (Physics.Raycast(sensorStartPos, Quaternion.AngleAxis(-frontSensorAngle * 3, Vector3.up) * transform.forward, out hit, sensorLength / 2))
        {
            Debug.DrawLine(sensorStartPos, hit.point);
            avoiding = true;
            avoidMultiplier += 0.25f;
        }

        //Front center sensor
        if (avoidMultiplier == 0)
        {
            if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
            {
                Debug.DrawLine(sensorStartPos, hit.point);
                avoiding = true;
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

        if (avoiding)
        {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }

        if (avoiding && currentSpeed > maxSpeed * 0.25f)
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }
    }

    private void ApplySteer()
    {
        if (avoiding)
            return;

        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;

        targetSteerAngle = newSteer;
    }

    private void Drive()
    {
        currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;

        if (currentSpeed < maxSpeed && !isBraking)
        {
            if (!isReversing)
            {
                wheelFL.motorTorque = maxMotorTorque;
                wheelFR.motorTorque = maxMotorTorque;
            }
            else
            {
                wheelFL.motorTorque = -maxMotorTorque * 0.5f;
                wheelFR.motorTorque = -maxMotorTorque * 0.5f;
            }
        }
        else
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
        }
    }

    IEnumerator ReverseCoroutine()
    {
        // Start reversing
        isReversing = true;
        slowTimeCounter = 0f;

        // Reverse for the specified duration
        yield return new WaitForSeconds(reversingDuration);

        // Stop reversing
        isReversing = false;
    }


    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, nodes[currentNode].position) < 1f)
        {
            if (currentNode == nodes.Count - 1)
            {
                currentNode = 0;
            }
            else
            {
                ++currentNode;
            }
        }
    }

    private void Braking()
    {
        if (isBraking)
        {
            wheelRL.brakeTorque = maxBrakeTorque;
            wheelRR.brakeTorque = maxBrakeTorque;
        }
        else
        {
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }

    private void LerpToSteerAngle()
    {
        if (isReversing)
            targetSteerAngle = -targetSteerAngle;

        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
    }

    private void CheckIfSlow()
    {
        if (currentSpeed < slowSpeedThreshold && !isBraking)
        {
            // Increment the slow time counter if the car is slow
            slowTimeCounter += Time.deltaTime;

            // If the car has been slow for the required duration, trigger reverse
            if (slowTimeCounter >= slowSpeedDuration)
            {
                StartCoroutine(ReverseCoroutine());
                slowTimeCounter = 0f;  // Reset the counter after reversing is triggered
            }
        }
        else
        {
            // Reset the counter if the car is not slow
            slowTimeCounter = 0f;
        }
    }
}
