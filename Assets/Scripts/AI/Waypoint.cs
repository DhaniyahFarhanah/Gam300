using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public enum State
    {
        Green,
        Yellow,
        Red
    }
    public State WaypointState = State.Green;
}
