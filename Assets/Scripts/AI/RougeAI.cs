using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RougeAI : AICarEngine
{
    [Header("Rouge AI")]
    public NodeGraph nodeGraph;
    public bool slowWhenAvoiding = true;
    public bool slowWhenTurning = true;
    private GameObject player;
    public float waypointBuffer = 3f;
    private List<Transform> waypoints = new List<Transform>();
    private int currentWaypoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        player = GameObject.FindGameObjectWithTag("Player");

        // Start the coroutine that waits 1 second and then starts pathfinding
        StartCoroutine(DelayedPathfinding());
    }

    private IEnumerator DelayedPathfinding()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(1f);

        // Perform pathfinding after the delay
        waypoints = nodeGraph.Pathfind(transform.position, player.transform.position);
        targetPosition = waypoints[currentWaypoint].position;

        // Check if waypoints were found and update the stop flag accordingly
        if (waypoints == null || waypoints.Count == 0)
        {
            stop = true;
            Debug.LogWarning("No waypoints found.");
        }
        else
        {
            stop = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EngineUpdate();
        CheckWaypointDistance();  // Check if the car is near the current waypoint
    }

    private void CheckWaypointDistance()
    {
        if (waypoints == null || waypoints.Count == 0) return; // Ensure waypoints are valid

        if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < waypointBuffer)
        {
            if (currentWaypoint == waypoints.Count - 1)
            {
                currentWaypoint = 0;  // Loop back to the first node when all waypoints are reached
                targetPosition = player.transform.position;
            }
            else
            {
                ++currentWaypoint;  // Move to the next waypoint
                targetPosition = waypoints[currentWaypoint].position;
            }

        }
    }
}
