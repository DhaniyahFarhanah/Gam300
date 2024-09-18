using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Path : MonoBehaviour
{
    public Color lineColor;
    public float sphereSize = 0.5f;
    public List<Transform> waypoints = new List<Transform>();

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        Transform[] pathTransform = GetComponentsInChildren<Transform>();
        waypoints = new List<Transform>();

        for (int i = 0; i < pathTransform.Length; ++i)
        {
            // Ensure we're not checking the parent object itself
            if (pathTransform[i] != transform)
            {
                // Check if the GameObject has the "Waypoint" tag
                if (pathTransform[i].CompareTag("Waypoint"))
                {
                    // Add the Transform to the nodes list
                    waypoints.Add(pathTransform[i]);
                }
            }
        }

        for (int i = 0; i < waypoints.Count; ++i)
        {
            Vector3 currentNode = waypoints[i].position;
            Vector3 previousNode = Vector3.zero;

            if (i > 0)
            {
                previousNode = waypoints[i - 1].position;
            }
            else if (i == 0 && waypoints.Count > 1)
            {
                previousNode = waypoints[waypoints.Count - 1].position;
            }

            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, sphereSize);
        }
    }
}
