using ArcadeVehicleController;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class BusStopSpawn : MonoBehaviour
{
    [SerializeField] private int m_PassengerAmt;
    [SerializeField] private Transform m_SpawnPoint;
    [SerializeField] private GameObject m_PassengerPrefab;
    [SerializeField] private List<GameObject> m_Passengers;

    [SerializeField] private float m_xrange;
    [SerializeField] private float m_zrange;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPassengers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPassengers()
    {
        for (int i = 0; i < m_PassengerAmt; i++)
        {
            float x = Random.Range(m_SpawnPoint.position.x - m_xrange, m_SpawnPoint.position.x + m_xrange);
            float z = Random.Range(m_SpawnPoint.position.z - m_zrange, m_SpawnPoint.position.z + m_zrange);
            m_Passengers.Add(Instantiate(m_PassengerPrefab, new Vector3(x, m_SpawnPoint.position.y, z), Quaternion.Euler(0.0f, Random.Range(0f, 360.0f),0.0f), m_SpawnPoint));
        }
    }

    public void RemovePassengers()
    {
        m_Passengers.RemoveAll(s => s == null);
    }

    public int CheckPassengers()
    {
        int num = 0;
        foreach(GameObject passenger in m_Passengers)
        {
            if (passenger != null)
            {
                num++;
            }
        }
        return num;
    }


    public IEnumerator Delay(GameObject bus)
    {
        foreach (GameObject passenger in m_Passengers)
        {
            passenger.GetComponent<Passenger>().m_PickedUp = true;
            passenger.GetComponent<Passenger>().m_target = bus;
            bus.GetComponent<BusAudioHandler>().Play(bus.GetComponent<BusAudioHandler>().passengerPop); 
            yield return new WaitForSeconds(0.2f);
        }
    }

    

}
