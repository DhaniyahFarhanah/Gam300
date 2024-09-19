using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverturnChecker : MonoBehaviour
{
    [SerializeField] private Vehicle m_Bus;
    [SerializeField] private GameObject m_BusVisual;
    [SerializeField] private GameObject m_CloudSpawn;

    private Vector3 m_BusRotation;
    private Vector3 m_BusTransform;
 
    // Start is called before the first frame update
    void Start()
    {
        m_Bus = GetComponent<Vehicle>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 eulers = this.transform.rotation.eulerAngles;
        Debug.Log(m_Bus.GetComponent<Rigidbody>().velocity.magnitude);

        if (Input.GetKeyUp(KeyCode.R)) 
        { 
            //flip bus back
            StartCoroutine(FlipBusBack());
        }
                
        else if((eulers.z >= 90f || eulers.z <= -90f || eulers.x >= 90f || eulers.x <= -90f) && m_Bus.GetComponent<Rigidbody>().velocity.magnitude <= 0.0001f)
        {

            Debug.Log("Overturned");
            m_Bus.GetComponent<Rigidbody>().velocity = Vector3.zero;
            StartCoroutine(FlipBusBack());
        }
    }

    IEnumerator FlipBusBack()
    {
        yield return new WaitForSeconds(0.2f);
        m_Bus.enabled = false;
        m_BusRotation = transform.rotation.eulerAngles;
        m_BusTransform = transform.position;
        m_BusVisual.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        transform.rotation = Quaternion.Euler(new Vector3(m_BusRotation.x, m_BusRotation.y, 0.0f));
        transform.position = new Vector3(m_BusTransform.x, m_BusTransform.y + 0.2f, m_BusTransform.z);
        m_BusVisual.SetActive(true);
        m_Bus.enabled = true;
    }
}
