using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficJunction : MonoBehaviour
{
    // List of traffic light objects to change color
    public float greenDuration = 10f;  // Duration for green light
    public float yellowDuration = 3f;  // Duration for yellow light
    [Range(0f, 1f)]
    public float yellowEarlyThreshold = 0.6f;
    public float redDuration = 10f;  // Duration for red light
    public float allRedDuration = 2f;  // Duration for all-red phase

    [Header("Road 1")]
    public List<Waypoint> lane1Waypoints;
    [Header("Road 2")]
    public List<Waypoint> lane2Waypoints;

    private void Start()
    {
        // Start the coroutine to control traffic light colors
        StartCoroutine(ChangeTrafficLightColors());
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = lineColor;
    //    Gizmos.DrawLine(Lane1Waypoint.transform.position, Lane2Waypoint.transform.position);
    //}

    // Coroutine to change traffic light colors
    IEnumerator ChangeTrafficLightColors()
    {
        while (true)
        {
            // Lane 1: Green, Lane 2: Red
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Green);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(greenDuration);

            // Lane 1: Yellow Early, Lane 2: Red
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.YellowEarly);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

            // Lane 1: Yellow Late, Lane 2: Red
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.YellowLate);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

            // All lights red (All-red phase)
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(allRedDuration);

            // Lane 1: Red, Lane 2: Green
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Green);  // Green for Lane 1
            }
            yield return new WaitForSeconds(greenDuration);

            // Lane 1: Red, Lane 2: Yellow Early
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.YellowEarly);  // Green for Lane 1
            }
            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

            // Lane 1: Red, Lane 2: Yellow Late
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.YellowLate);  // Green for Lane 1
            }
            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

            // All lights red (All-red phase)
            foreach (Waypoint waypoint in lane1Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            foreach (Waypoint waypoint in lane2Waypoints)
            {
                if (waypoint != null)
                    SetLaneState(waypoint, Waypoint.State.Red);  // Green for Lane 1
            }
            yield return new WaitForSeconds(allRedDuration);
        }
    }

    // Helper function to set colors of traffic lights in a lane
    void SetLaneState(Waypoint node, Waypoint.State state)
    {
        if (node != null)
        {
            node.SetState(state);
        }
    }
}
