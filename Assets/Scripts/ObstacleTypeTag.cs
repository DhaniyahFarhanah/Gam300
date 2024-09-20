using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleType : MonoBehaviour
{
    public enum ObstacleTag
    {
        Light,
        Medium,
        Heavy,
        Pedestrian
    }

    public ObstacleTag obstacleTag;
}
