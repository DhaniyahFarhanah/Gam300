using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PickupPassengers : MonoBehaviour
{
    private List<GameObject> passengers = new List<GameObject>();

    public void PickupPassenger(GameObject passenger)
    {
        passengers.Add(passenger);

        passenger.SetActive(false);

        Debug.Log("Picked up, Current people count: " + passengers.Count);
    }

    public void EjectPassenger(EjectPoint ejectPoint)
    {
        foreach (GameObject passenger in passengers)
        {
            passenger.SetActive(true);

            passenger.transform.position = ejectPoint.transform.position
                + ejectPoint.transform.forward * UnityEngine.Random.Range(5f, 4f)
                + ejectPoint.transform.right * UnityEngine.Random.Range(-3f, 3f)
                + new Vector3(0, 2, 0); 

            Debug.Log("Ejected");
        }

        passengers.Clear();
    }
}
