using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverturnChecker : MonoBehaviour
{
    [SerializeField] private Vehicle m_Bus;
    [SerializeField] private GameObject m_BusVisual;
    [SerializeField] private GameObject m_CloudSpawn;

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

        if (Input.GetKeyUp(KeyCode.R)) 
        { 
            //flip bus back
            StartCoroutine(FlipBusBack());
        }
        
        else if((eulers.z > 90f || eulers.z < -90f) && m_Bus.Velocity.magnitude <= 0.1f)
        {
            StartCoroutine(FlipBusBack());
        }
    }

    IEnumerator FlipBusBack()
    {
        yield return new WaitForSeconds(0.2f);
        m_BusTransform = transform.rotation.eulerAngles;
        m_Bus.enabled = false;
        m_BusVisual.SetActive(false);
        transform.rotation = Quaternion.Euler(new Vector3(m_BusTransform.x, m_BusTransform.y, 0.0f));
        yield return new WaitForSeconds(0.5f);
        m_BusVisual.SetActive(true);
        transform.rotation = Quaternion.Euler(new Vector3(m_BusTransform.x, m_BusTransform.y, 0.0f));
        m_Bus.enabled = true;
    }
}
