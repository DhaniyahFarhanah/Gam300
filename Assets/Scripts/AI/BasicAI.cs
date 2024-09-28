using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : BaseAI
{
    [Header("Basic AI")]
    public Path path;
    public bool slowWhenAvoiding = true;
    public bool slowWhenTurning = true;
    public float waypointBuffer = 3f;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypoint = 0;
    private enum AIState
    {
        DrivingNormal,
        AvoidingObstacle,
        StopVehicleAhead
    }
    private AIState State = AIState.DrivingNormal;

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
}
