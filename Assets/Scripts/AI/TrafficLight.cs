using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    // List of traffic light objects to change color
    public float greenDuration = 10f;  // Duration for green light
    public float yellowDuration = 3f;  // Duration for yellow light
    public float yellowEarlyThreshold = 0.6f;
    public float redDuration = 10f;  // Duration for red light
    public float allRedDuration = 2f;  // Duration for all-red phase
    public Waypoint Lane1Waypoint;
    public Waypoint Lane2Waypoint;
    public GameObject lightPrefab;
    public Color lineColor;
    private GameObject lane1Light;  // Lights for Lane 1
    private GameObject lane2Light;  // Lights for Lane 2

    private Color[] colors;  // Array of colors (green, yellow, red)

    private void Start()
    {
        // Store the colors in an array
        colors = new Color[] { Color.green, Color.yellow, Color.red };

        lane1Light = Instantiate(lightPrefab, Lane1Waypoint.transform);
        lane2Light = Instantiate(lightPrefab, Lane2Waypoint.transform);

        // Start the coroutine to control traffic light colors
        StartCoroutine(ChangeTrafficLightColors());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        Gizmos.DrawLine(Lane1Waypoint.transform.position, Lane2Waypoint.transform.position);
    }

    // Coroutine to change traffic light colors
    IEnumerator ChangeTrafficLightColors()
    {
        while (true)
        {
            // Lane 1: Green, Lane 2: Red
            SetLaneColors(lane1Light, colors[0], Lane1Waypoint, Waypoint.State.Green);  // Green for Lane 1
            SetLaneColors(lane2Light, colors[2], Lane2Waypoint, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(greenDuration);

            // Lane 1: Yellow Early, Lane 2: Red
            SetLaneColors(lane1Light, colors[1], Lane1Waypoint, Waypoint.State.YellowEarly);  // Yellow Early for Lane 1
            SetLaneColors(lane2Light, colors[2], Lane2Waypoint, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

            // Lane 1: Yellow Late, Lane 2: Red
            SetLaneColors(lane1Light, colors[1], Lane1Waypoint, Waypoint.State.YellowLate);  // Yellow Late for Lane 1
            SetLaneColors(lane2Light, colors[2], Lane2Waypoint, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

            // All lights red (All-red phase)
            SetLaneColors(lane1Light, colors[2], Lane1Waypoint, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(lane2Light, colors[2], Lane2Waypoint, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(allRedDuration);

            // Lane 1: Red, Lane 2: Green
            SetLaneColors(lane1Light, colors[2], Lane1Waypoint, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(lane2Light, colors[0], Lane2Waypoint, Waypoint.State.Green);  // Green for Lane 2
            yield return new WaitForSeconds(greenDuration);

            // Lane 1: Red, Lane 2: Yellow Early
            SetLaneColors(lane1Light, colors[2], Lane1Waypoint, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(lane2Light, colors[1], Lane2Waypoint, Waypoint.State.YellowEarly);  // Yellow Early for Lane 2
            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

            // Lane 1: Red, Lane 2: Yellow Late
            SetLaneColors(lane1Light, colors[2], Lane1Waypoint, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(lane2Light, colors[1], Lane2Waypoint, Waypoint.State.YellowLate);  // Yellow Late for Lane 2
            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

            // All lights red (All-red phase)
            SetLaneColors(lane1Light, colors[2], Lane1Waypoint, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(lane2Light, colors[2], Lane2Waypoint, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(allRedDuration);
        }
    }

    // Helper function to set colors of traffic lights in a lane
    void SetLaneColors(GameObject lane, Color color, Waypoint node, Waypoint.State state)
    {
        lane.GetComponent<Renderer>().material.color = color;
        node.WaypointState = state;
    }
}
