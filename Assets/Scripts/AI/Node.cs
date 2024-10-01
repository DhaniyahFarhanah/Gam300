using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    public List<GameObject> neighbours;
    public bool drawLine = false;

    private void Awake()
    {
        foreach (GameObject obj in neighbours)
        {
            if (!obj.GetComponent<Node>().InNeighours(gameObject))
                obj.GetComponent<Node>().neighbours.Add(gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        if (drawLine)
        {
            foreach (GameObject obj in neighbours)
            {
                Debug.DrawLine(transform.position, obj.transform.position);
            }
        }
    }

    private bool InNeighours(GameObject obj1)
    {
        foreach (GameObject obj2 in neighbours)
        {
            if (obj1 == obj2)
                return true;
        }
        return false;
    }
}
