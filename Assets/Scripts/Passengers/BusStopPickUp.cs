using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusStopPickUp : MonoBehaviour
{
    [SerializeField] private BusStopSpawn m_BusStop;
    [SerializeField] private MeshRenderer m_Box;
    [SerializeField] private float m_Timer;

    private float m_CurrentTimer;

    private bool m_PickUp;
    private Vehicle m_bus;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentTimer = m_Timer;
        m_PickUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_BusStop == null)
            return;

        if (m_bus == null)
            return;

        //countdown
        if (m_PickUp && m_bus.Velocity.magnitude <= 3f)
        {
            if (m_CurrentTimer <= 0f)
            {
                StartCoroutine(m_BusStop.Delay(m_bus.gameObject));
                m_PickUp = false;
                m_Box.enabled = false;
                m_bus.TimerCanvas.SetActive(false);
                m_CurrentTimer = m_Timer;
            }
            else
            {
                m_CurrentTimer -= Time.deltaTime;
                m_bus.CountdownPickUp(m_CurrentTimer, m_Timer, Color.green);
            }
        }
        else
        {
            if (m_CurrentTimer < m_Timer)
            {
                m_CurrentTimer += Time.deltaTime;
                m_bus.CountdownPickUp(m_CurrentTimer, m_Timer, Color.red);
            }

        }

        if (m_BusStop.CheckPassengers() <= 0)
        {
           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            m_PickUp = true;
            m_bus = other.GetComponent<Vehicle>();

            if (m_BusStop.CheckPassengers() <= 0)
            {
                m_bus.TimerCanvas.SetActive(false);
            }
            else
            {
                m_bus.TimerCanvas.SetActive(true);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Stop countdown and restart
            m_PickUp = false;
            m_CurrentTimer = m_Timer;
            m_bus.ResetPickUpCountdown();
            m_bus.TimerCanvas.SetActive(false);
        }
    }
}
