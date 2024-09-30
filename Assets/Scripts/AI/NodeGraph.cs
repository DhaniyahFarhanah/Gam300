using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGraph : MonoBehaviour
{
    public List<Node> nodesList = new List<Node>();

    private void Start()
    {
        Node[] childNode = GetComponentsInChildren<Node>();
        for (int i = 0; i < childNode.Length; ++i)
        {
            nodesList.Add(childNode[i]);
        }
    }

    public GameObject myObject; // Object that will move
    public GameObject target; // The target object to find

    private Node startNode; // Starting node (nearest to myObject)
    private Node targetNode; // Target node (nearest to target)

    // This function finds the nearest node to a given position
    private Node GetNearestNode(Vector3 position)
    {
        Node nearestNode = null;
        float minDistance = Mathf.Infinity;

        foreach (Node node in nodesList)
        {
            float distance = Vector3.Distance(position, node.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestNode = node;
            }
        }

        Debug.Log("Nearest Node: " + nearestNode.name);
        return nearestNode;
    }

    // Breadth-First Search (BFS) to find the path
    private List<Node> BFS(Node start, Node target)
    {
        Queue<Node> queue = new Queue<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        queue.Enqueue(start);
        cameFrom[start] = null; // Start node has no parent

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();

            // If we found the target node
            if (currentNode == target)
            {
                List<Node> path = new List<Node>();
                Node current = target;

                // Reconstruct path by tracing back the parents
                while (current != null)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }

                path.Reverse(); // Reverse the path to get the correct order
                return path;
            }

            // Enqueue all neighbors if they haven't been visited
            foreach (GameObject neighborObj in currentNode.neighbours)
            {
                Node neighbor = neighborObj.GetComponent<Node>();
                if (!cameFrom.ContainsKey(neighbor))
                {
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = currentNode;
                }
            }
        }

        return null; // No path found
    }

    public List<Transform> Pathfind(Vector3 start, Vector3 end)
    {
        // Get the nearest nodes to the start and end positions
        startNode = GetNearestNode(start);
        targetNode = GetNearestNode(end);

        // Perform BFS to find the path
        List<Node> path = BFS(startNode, targetNode);

        // Check if a path was found
        if (path == null)
        {
            Debug.LogWarning("No path found between the start and target nodes.");
            return null;
        }

        // Convert the path of Node objects to a list of Transforms
        List<Transform> ret = new List<Transform>();

        foreach (Node node in path)
        {
            ret.Add(node.gameObject.transform);
        }

        Debug.Log("Path found with " + ret.Count + " waypoints.");
        return ret;
    }
}
