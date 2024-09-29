using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverturnChecker : MonoBehaviour
{
    [SerializeField] private float buffer = 5f;

    [SerializeField] private Vehicle m_Bus;
    [SerializeField] private GameObject m_BusVisual;
    [SerializeField] private GameObject m_CloudSpawn;

    private Vector3 m_BusRotation;
    private Vector3 m_BusTransform;
    private bool m_Overturned;
 
    // Start is called before the first frame update
    void Start()
    {
        m_Bus = GetComponent<Vehicle>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 eulers = this.transform.rotation.eulerAngles;

        if (Input.GetKeyUp(KeyCode.R)) 
        { 
            //flip bus back
            StartCoroutine(FlipBusBack());
        }
                
        else if((eulers.z >= 90f - buffer || eulers.z <= -90f + buffer || eulers.x >= 90f - buffer || eulers.x <= -90f + buffer) && m_Bus.GetComponent<Rigidbody>().velocity.magnitude <= 0.001f)
        {
            m_Bus.GetComponent<Rigidbody>().velocity = Vector3.zero;
            
            m_Overturned = true;
        }

        if (m_Overturned)
        {
            StartCoroutine(FlipBusBack());
            m_Overturned = false;
        }
    }

    IEnumerator FlipBusBack()
    {
        m_BusRotation = transform.rotation.eulerAngles;
        m_BusTransform = transform.position;
        m_BusVisual.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        m_Bus.enabled = false;
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, m_BusRotation.y, 0.0f));
        transform.position = new Vector3(m_BusTransform.x, m_BusTransform.y + 0.2f, m_BusTransform.z);
        yield return new WaitForSeconds(0.2f);
        m_Bus.enabled = true;
        m_BusVisual.SetActive(true);
    }
}
