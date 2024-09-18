using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EjectPoint : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PickupPassengers>().EjectPassenger(this);
        }
    }
}
