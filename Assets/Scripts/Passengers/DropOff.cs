using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOff : MonoBehaviour
{
    [SerializeField] private GameObject m_PassengerPrefab;

    private float m_CurrentTimer;

    private bool m_DropOff;
    private Vehicle m_bus;
    private int m_PassengerCount;

    // Start is called before the first frame update
    void Start()
    {
        m_bus = GameObject.FindGameObjectWithTag("Player").GetComponent<Vehicle>();
        m_DropOff = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bus == null)
            return;

        if (m_DropOff && m_bus.Velocity.magnitude <= 3f)
        {
            if (m_CurrentTimer <= 0f)
            {
                m_DropOff = false;
                m_bus.TimerCanvas.SetActive(false);
                m_bus.ThrowPassengers(true);
            }
            else
            {
                m_CurrentTimer -= Time.deltaTime;
                m_bus.CountdownPickUp(m_CurrentTimer, m_PassengerCount * 0.1f, Color.green);
            }
        }
        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_PassengerCount = m_bus.m_Passengers;
            if(m_PassengerCount > 0)
            {
                m_DropOff = true;
                m_bus.TimerCanvas.SetActive(true);
                m_CurrentTimer = m_PassengerCount * 0.1f;
            }
            else
            {
                m_DropOff = false;
                m_bus.TimerCanvas.SetActive(false);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Stop countdown and restart
            m_DropOff = false;
            m_bus.ResetPickUpCountdown();
            m_bus.TimerCanvas.SetActive(false);
        }
    }
}
