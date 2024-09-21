using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private CameraShake m_CamShake;
    private ObstacleTag m_ObstacleType;
    // Start is called before the first frame update
    void Start()
    {
        m_CamShake = Camera.main.GetComponent<CameraShake>();

        if (m_CamShake == null)
            return;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        ObstacleType obs = collision.gameObject.GetComponent<ObstacleType>();

        if(obs != null)
        {
            m_ObstacleType = obs.obstacleTag;
            Debug.Log("Collided with " + m_ObstacleType);
        }
    }
}
