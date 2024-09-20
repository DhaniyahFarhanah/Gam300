using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EjectPointHandler : MonoBehaviour
{
    [SerializeField] GameObject[] m_DropOffPoints;
    // Start is called before the first frame update
    void Start()
    {
        m_DropOffPoints = GameObject.FindGameObjectsWithTag("DropOff");

    }

    // Update is called once per frame
    void Update()
    {
        if(m_DropOffPoints != null)
        {
            transform.LookAt(ClosestDropOff().transform);
        }
        
    }

    GameObject ClosestDropOff()
    {
        GameObject tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject dropOff in m_DropOffPoints)
        {
            float dist = Vector3.Distance(dropOff.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = dropOff;
                minDist = dist;
            }
        }

        return tMin;
    }
}
