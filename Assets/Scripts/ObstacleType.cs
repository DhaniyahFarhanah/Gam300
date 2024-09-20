using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleType : MonoBehaviour
{
    public enum ObstacleTag
    {
        None,
        Light,
        Medium,
        Heavy,
        Pedestrian,
        CarAI,
        Player
    }

    public ObstacleTag obstacleTag;
}
