using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArcadeVehicleController;

public class Toilet : MonoBehaviour
{

    public IEnumerator GoToilet(Vehicle bus)
    {
        bus.GetComponent<PoopMeter>().ReducePoop(10f);

        yield return null;
    }
}
