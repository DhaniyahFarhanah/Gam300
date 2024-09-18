using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpPoint : MonoBehaviour
{
    [SerializeField]
    GameObject[] passengers;

    bool pickedUp = false;

    void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;
        if (other.tag == "Player")
        {
            pickedUp = true;
            foreach (GameObject passenger in passengers)
            {
                other.gameObject.GetComponent<PickupPassengers>().PickupPassenger(passenger);
            }
        }
    }
}
