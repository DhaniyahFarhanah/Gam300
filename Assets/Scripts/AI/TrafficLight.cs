using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    // List of traffic light objects to change color
    public List<GameObject> Lane1;  // Lights for Lane 1
    public List<GameObject> Lane2;  // Lights for Lane 2
    public float greenDuration = 10f;  // Duration for green light
    public float yellowDuration = 3f;  // Duration for yellow light
    public float redDuration = 10f;  // Duration for red light
    public float allRedDuration = 2f;  // Duration for all-red phase
    public List<Waypoint> Lane1Nodes;
    public List<Waypoint> Lane2Nodes;

    private Color[] colors;  // Array of colors (green, yellow, red)

    private void Start()
    {
        // Store the colors in an array
        colors = new Color[] { Color.green, Color.yellow, Color.red };

        // Start the coroutine to control traffic light colors
        StartCoroutine(ChangeTrafficLightColors());
    }

    // Coroutine to change traffic light colors
    IEnumerator ChangeTrafficLightColors()
    {
        while (true)
        {
            // Lane 1: Green, Lane 2: Red
            SetLaneColors(Lane1, colors[0], Lane1Nodes, Waypoint.State.Green);  // Green for Lane 1
            SetLaneColors(Lane2, colors[2], Lane2Nodes, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(greenDuration);

            // Lane 1: Yellow, Lane 2: Red
            SetLaneColors(Lane1, colors[1], Lane1Nodes, Waypoint.State.Yellow);  // Yellow for Lane 1
            SetLaneColors(Lane2, colors[2], Lane2Nodes, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(yellowDuration);

            // All lights red (All-red phase)
            SetLaneColors(Lane1, colors[2], Lane1Nodes, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(Lane2, colors[2], Lane2Nodes, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(allRedDuration);

            // Lane 1: Red, Lane 2: Green
            SetLaneColors(Lane1, colors[2], Lane1Nodes, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(Lane2, colors[0], Lane2Nodes, Waypoint.State.Green);  // Green for Lane 2
            yield return new WaitForSeconds(greenDuration);

            // Lane 1: Red, Lane 2: Yellow
            SetLaneColors(Lane1, colors[2], Lane1Nodes, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(Lane2, colors[1], Lane2Nodes, Waypoint.State.Yellow);  // Yellow for Lane 2
            yield return new WaitForSeconds(yellowDuration);

            // All lights red (All-red phase)
            SetLaneColors(Lane1, colors[2], Lane1Nodes, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(Lane2, colors[2], Lane2Nodes, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(allRedDuration);
        }
    }

    // Helper function to set colors of traffic lights in a lane
    void SetLaneColors(List<GameObject> lane, Color color, List<Waypoint> nodes, Waypoint.State state)
    {
        foreach (GameObject light in lane)
        {
            light.GetComponent<Renderer>().material.color = color;
        }
        foreach (Waypoint node in nodes)
        {
            node.WaypointState = state;
        }
    }
}
