using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeVehicleController;

public class ToiletPickUp : MonoBehaviour
{
    [SerializeField] private Toilet m_toilet;
    [SerializeField] private MeshRenderer m_Box;
    [SerializeField] private float m_Timer;

    private float m_CurrentTimer;

    private bool inArea;
    private Vehicle m_bus;

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentTimer = m_Timer;
        inArea = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inArea && m_bus.Velocity.magnitude <= 3f)
        {
            if (m_CurrentTimer <= 0f)
            {
                m_toilet.GoToilet(m_bus);
                inArea = false;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inArea = true;
            m_bus = other.GetComponent<Vehicle>();
            m_bus.TimerCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Stop countdown and restart
            inArea = false;
            m_CurrentTimer = m_Timer;
            m_bus.ResetPickUpCountdown();
            m_bus.TimerCanvas.SetActive(false);
        }
    }
}
