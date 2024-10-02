using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : AICarEngine
{
    [Header("Basic AI")]
    public Path path;
    public bool slowWhenAvoiding = true;
    public bool slowWhenTurning = true;
    public float waypointBuffer = 3f;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypoint = 0;
    public enum AIState
    {
        DrivingNormal,
        AvoidingObstacle,
        StopVehicleAhead
    }
    public AIState State = AIState.DrivingNormal;

    [Header("Traffic")]
    public float stoppingDistance = 1f;
    public float decelerationDistance = 5f;

    // Start is called before the first frame update
    private void Start()
    {
        base.Init();
        waypoints = path.waypoints;
        FindNearestNode();
    }

    private void FixedUpdate()
    {
        EngineUpdate();
        CheckWaypointDistance();  // Check if the car is near the current waypoint
    }

    #region Route
    private void FindNearestNode()
    {
        float nearestDistance = Mathf.Infinity;  // Set an initially large value for comparison
        int nearestNodeIndex = 0;  // Variable to store the index of the nearest node

        // Loop through all nodes
        for (int i = 0; i < waypoints.Count; i++)
        {
            // Calculate the distance between the car and the current node
            float distance = Vector3.Distance(transform.position, waypoints[i].position);

            // If the current node is closer than the previously found nearest node
            if (distance < nearestDistance)
            {
                nearestDistance = distance;  // Update the nearest distance
                nearestNodeIndex = i;  // Update the nearest node index
            }
        }

        // Set the nearest node as the current node the car should travel to
        currentWaypoint = nearestNodeIndex;
        targetPosition = waypoints[currentWaypoint].position;
    }

    private void CheckWaypointDistance()
    {
        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < waypointBuffer)
        {
            if (currentWaypoint == waypoints.Count - 1)
            {
                currentWaypoint = 0;  // Loop back to the first node when all waypoints are reached
            }
            else
            {
                ++currentWaypoint;  // Move to the next waypoint
            }
            targetPosition = waypoints[currentWaypoint].position;
        }
    }
    #endregion Route

    protected override void ObstacleResponse()
    {
        if (!detectedObstacle)
        {
            State = AIState.DrivingNormal;
        }
        else
        {
            ObstacleTag sensedObstacleTag = detectedObstacleHit.collider.gameObject.GetComponent<ObstacleType>().obstacleTag;

            if (sensedObstacleTag == ObstacleTag.Light || sensedObstacleTag == ObstacleTag.Medium || sensedObstacleTag == ObstacleTag.Heavy || sensedObstacleTag == ObstacleTag.Pedestrian)
            {
                State = AIState.AvoidingObstacle;
            }
            else if (sensedObstacleTag == ObstacleTag.CarAI || sensedObstacleTag == ObstacleTag.Player)
            {
                if (Vector3.Dot(transform.forward, detectedObstacleHit.collider.gameObject.transform.forward) > 0f)
                {
                    State = AIState.StopVehicleAhead;
                }
                else
                {
                    State = AIState.AvoidingObstacle;
                }
            }
            else if (sensedObstacleTag == ObstacleTag.None)
            {
                State = AIState.DrivingNormal;
            }
        }
    }

    protected override void BrakeLogic()
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
        else if (State == AIState.StopVehicleAhead && detectedObstacleHit.distance < decelerationDistance && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }
        else if (State == AIState.StopVehicleAhead && detectedObstacleHit.distance < stoppingDistance)
        {
            isBraking = true;
        }

        Vector3 carForwardPosition = transform.position;
        carForwardPosition += transform.forward * frontSensorPosition.z;
        carForwardPosition += transform.up * frontSensorPosition.y;
        float distanceToLight = Vector3.Distance(carForwardPosition, waypoints[currentWaypoint].position);

        switch (waypoints[currentWaypoint].GetComponent<Waypoint>().GetState())
        {
            case Waypoint.State.Green:
                break;
            case Waypoint.State.YellowEarly:
                if (distanceToLight < decelerationDistance && currentSpeed > maxSpeed * highSpeedThreshold * 0.5f)
                {
                    isBraking = true;
                }
                break;
            case Waypoint.State.YellowLate:
            case Waypoint.State.Red:
                if (distanceToLight < stoppingDistance)
                {
                    isBraking = true;
                }
                else if (distanceToLight < decelerationDistance && currentSpeed > maxSpeed * highSpeedThreshold * 0.5f)
                {
                    isBraking = true;
                }
                break;
        }
    }

    protected override void ApplySteer()
    {
        if (State == AIState.AvoidingObstacle)
        {
            targetSteerAngle = maxSteerAngle * avoidMultiplier;
        }
        else
        {
            if (debugLine)
                Debug.DrawLine(transform.position, targetPosition, targetLineColor);

            Vector3 relativeVector = transform.InverseTransformPoint(targetPosition);
            float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
            targetSteerAngle = newSteer;
        }
    }
}
