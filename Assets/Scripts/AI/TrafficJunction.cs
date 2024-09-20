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
    public GameObject lightPrefab;
    //public Color lineColor;

    [Header("Road 1")]
    public Waypoint road1Waypoint1;
    public Waypoint road1Waypoint2;
    [Header("Road 2")]
    public Waypoint road2Waypoint1;
    public Waypoint road2Waypoint2;

    private GameObject road1Light1;
    private GameObject road1Light2;
    private GameObject road2Light1;
    private GameObject road2Light2;

    private Color[] colors;  // Array of colors (green, yellow, red)

    private void Start()
    {
        // Store the colors in an array
        colors = new Color[] { Color.green, Color.yellow, Color.red };

        road1Light1 = Instantiate(lightPrefab, road1Waypoint1.transform);
        road1Light2 = Instantiate(lightPrefab, road1Waypoint2.transform);
        road2Light1 = Instantiate(lightPrefab, road2Waypoint1.transform);
        road2Light2 = Instantiate(lightPrefab, road2Waypoint2.transform);

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
            SetLaneColors(road1Light1, colors[0], road1Waypoint1, Waypoint.State.Green);  // Green for Lane 1
            SetLaneColors(road1Light2, colors[0], road1Waypoint2, Waypoint.State.Green);  // Green for Lane 1
            SetLaneColors(road2Light1, colors[2], road2Waypoint1, Waypoint.State.Red);  // Red for Lane 2
            SetLaneColors(road2Light2, colors[2], road2Waypoint2, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(greenDuration);

            // Lane 1: Yellow Early, Lane 2: Red
            SetLaneColors(road1Light1, colors[1], road1Waypoint1, Waypoint.State.YellowEarly);  // Yellow for Lane 1
            SetLaneColors(road1Light2, colors[1], road1Waypoint2, Waypoint.State.YellowEarly);  // Yellow for Lane 1
            SetLaneColors(road2Light1, colors[2], road2Waypoint1, Waypoint.State.Red);  // Red for Lane 2
            SetLaneColors(road2Light2, colors[2], road2Waypoint2, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

            // Lane 1: Yellow Late, Lane 2: Red
            SetLaneColors(road1Light1, colors[1], road1Waypoint1, Waypoint.State.YellowLate);  // Yellow for Lane 1
            SetLaneColors(road1Light2, colors[1], road1Waypoint2, Waypoint.State.YellowLate);  // Yellow for Lane 1
            SetLaneColors(road2Light1, colors[2], road2Waypoint1, Waypoint.State.Red);  // Red for Lane 2
            SetLaneColors(road2Light2, colors[2], road2Waypoint2, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

            // All lights red (All-red phase)
            SetLaneColors(road1Light1, colors[2], road1Waypoint1, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(road1Light2, colors[2], road1Waypoint2, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(road2Light1, colors[2], road2Waypoint1, Waypoint.State.Red);  // Red for Lane 2
            SetLaneColors(road2Light2, colors[2], road2Waypoint2, Waypoint.State.Red);  // Red for Lane 2
            yield return new WaitForSeconds(allRedDuration);

            // Lane 1: Red, Lane 2: Green
            SetLaneColors(road1Light1, colors[2], road1Waypoint1, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(road1Light2, colors[2], road1Waypoint2, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(road2Light1, colors[0], road2Waypoint1, Waypoint.State.Green);  // Green for Lane 2
            SetLaneColors(road2Light2, colors[0], road2Waypoint2, Waypoint.State.Green);  // Green for Lane 2
            yield return new WaitForSeconds(greenDuration);

            // Lane 1: Red, Lane 2: Yellow Early
            SetLaneColors(road1Light1, colors[2], road1Waypoint1, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(road1Light2, colors[2], road1Waypoint2, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(road2Light1, colors[1], road2Waypoint1, Waypoint.State.YellowEarly);  // Yellow for Lane 2
            SetLaneColors(road2Light2, colors[1], road2Waypoint2, Waypoint.State.YellowEarly);  // Yellow for Lane 2
            yield return new WaitForSeconds(yellowDuration * yellowEarlyThreshold);

            // Lane 1: Red, Lane 2: Yellow Late
            SetLaneColors(road1Light1, colors[2], road1Waypoint1, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(road1Light2, colors[2], road1Waypoint2, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(road2Light1, colors[1], road2Waypoint1, Waypoint.State.YellowLate);  // Yellow for Lane 2
            SetLaneColors(road2Light2, colors[1], road2Waypoint2, Waypoint.State.YellowLate);  // Yellow for Lane 2
            yield return new WaitForSeconds(yellowDuration * (1 - yellowEarlyThreshold));

            // All lights red (All-red phase)
            SetLaneColors(road1Light1, colors[2], road1Waypoint1, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(road1Light2, colors[2], road1Waypoint2, Waypoint.State.Red);  // Red for Lane 1
            SetLaneColors(road2Light1, colors[2], road2Waypoint1, Waypoint.State.Red);  // Red for Lane 2
            SetLaneColors(road2Light2, colors[2], road2Waypoint2, Waypoint.State.Red);  // Red for Lane 2
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
