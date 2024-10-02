using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public enum State
    {
        Green,
        YellowEarly,
        YellowLate,
        Red
    }
    private State WaypointState = State.Green;
    [SerializeField] TrafficLightDisplay trafficLight;

    public void SetState(State state)
    {
        WaypointState = state;
        if (trafficLight != null)
            trafficLight.DisplayColor(state);
    }

    public State GetState()
    {
        return WaypointState;
    }
}
