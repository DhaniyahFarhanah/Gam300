using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightDisplay : MonoBehaviour
{
    [SerializeField] private GameObject redLight;
    [SerializeField] private GameObject YellowLight;
    [SerializeField] private GameObject GreenLight;

    public void DisplayColor(Waypoint.State state)
    {
        switch (state)
        {
            case Waypoint.State.Green:
                redLight.GetComponent<Renderer>().material.color = Color.grey;
                YellowLight.GetComponent<Renderer>().material.color = Color.grey;
                GreenLight.GetComponent<Renderer>().material.color = Color.green;
                break;
            case Waypoint.State.YellowEarly:
            case Waypoint.State.YellowLate:
                redLight.GetComponent<Renderer>().material.color = Color.grey;
                YellowLight.GetComponent<Renderer>().material.color = Color.yellow;
                GreenLight.GetComponent<Renderer>().material.color = Color.grey;
                break;
            case Waypoint.State.Red:
                redLight.GetComponent<Renderer>().material.color = Color.red;
                YellowLight.GetComponent<Renderer>().material.color = Color.grey;
                GreenLight.GetComponent<Renderer>().material.color = Color.grey;
                break;
        }
    }
}
