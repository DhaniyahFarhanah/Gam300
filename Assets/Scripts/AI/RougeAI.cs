using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RougeAI : AICarEngine
{
    [Header("Rouge AI")]
    public NodeGraph nodeGraph;
    public bool slowWhenAvoiding = true;
    public bool slowWhenTurning = true;
    public float waypointBuffer = 3f;
    public float delayedStart = 60f;
    private GameObject player;
    private Node playerNearestNode;
    private List<Transform> waypoints = new List<Transform>();
    public enum AIState
    {
        enroute,
        ChasingPlayer,
    }
    public AIState State = AIState.enroute;
    private bool avoiding = false;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();

        player = GameObject.FindGameObjectWithTag("Player");
        playerNearestNode = nodeGraph.GetNearestNode(player.transform.position);
        stop = true;
        StartCoroutine(DelayedPathfinding());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EngineUpdate();
        CheckWaypointDistance();  // Check if the car is near the current waypoint
    }

    #region Pathfinding
    private IEnumerator DelayedPathfinding()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(delayedStart);
        FindAnyObjectByType<PoliceUI>().activatePoliceUI();
        stop = false;
        PathfindToPlayer(false);
    }

    private void PathfindToPlayer(bool removeFirst)
    {
        // Perform pathfinding after the delay
        waypoints = nodeGraph.Pathfind(transform.position, player.transform.position);

        // Check if waypoints were found and update the stop flag accordingly
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints found.");
        }

        if (waypoints.Count >= 2 && removeFirst)
        {
            waypoints.RemoveAt(0);
        }

        if (waypoints.Count > 0)
        {
            targetPosition = waypoints[0].position;
        }

        State = AIState.enroute;
    }

    private void CheckWaypointDistance()
    {
        // Player moved to a new node, re-pathfind to player
        Node currentNearestNode = nodeGraph.GetNearestNode(player.transform.position);
        if (playerNearestNode != currentNearestNode)
        {
            playerNearestNode = currentNearestNode;
            PathfindToPlayer(true);
        }

        if (waypoints.Count > 0)
        {
            // Check if we've reached the current waypoint
            if (Vector3.Distance(transform.position, waypoints[0].position) < waypointBuffer)
            {
                waypoints.RemoveAt(0);  // Remove reached waypoint
            }

            // Set target position based on waypoints or player position
            if (waypoints.Count > 0)
            {
                targetPosition = waypoints[0].position;

                // If the player is closer than the next waypoint, go towards the player instead
                if (Vector3.Distance(transform.position, player.transform.position) < Vector3.Distance(transform.position, targetPosition))
                {
                    targetPosition = player.transform.position;
                    State = AIState.ChasingPlayer;
                }
            }
            else
            {
                // No waypoints left, go directly to the player
                targetPosition = player.transform.position;
                State = AIState.ChasingPlayer;
            }
        }
        else
        {
            // No waypoints left, go directly to the player
            targetPosition = player.transform.position;
            State = AIState.ChasingPlayer;
        }
    }
    #endregion Pathfinding

    #region DrivingLogic
    protected override void ObstacleResponse()
    {
        if (!detectedObstacle)
        {
            avoiding = false;
        }
        else
        {
            ObstacleTag sensedObstacleTag = detectedObstacleHit.collider.gameObject.GetComponent<ObstacleType>().obstacleTag;

            if (State == AIState.enroute && sensedObstacleTag == ObstacleTag.Light || sensedObstacleTag == ObstacleTag.Medium || sensedObstacleTag == ObstacleTag.Heavy || sensedObstacleTag == ObstacleTag.CarAI)
            {
                avoiding = true;
            }
        }
    }

    protected override void BrakeLogic()
    {
        isBraking = false;
        if (slowWhenAvoiding && avoiding && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }
        else if (slowWhenTurning && Mathf.Abs(wheelFL.steerAngle) >= maxSteerAngle * sharpTurnThreshold && currentSpeed > maxSpeed * highSpeedThreshold)
        {
            isBraking = true;
        }
    }

    protected override void ApplySteer()
    {
        if (avoiding)
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
    #endregion DrivingLogic
}
