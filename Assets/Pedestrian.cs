using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestrian : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PickupPassengers>().PickupPassenger(gameObject);
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}
