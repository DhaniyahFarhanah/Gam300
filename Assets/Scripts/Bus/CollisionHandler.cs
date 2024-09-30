using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] private float m_CollisionCooldown;
    private CameraShake m_CamShake;

    [Header("Light Obstacle Camera Shake")]
    [SerializeField] private float m_LightDuration;
    [Range(0.00f, 1.00f)] [SerializeField] private float m_LightIntensity;

    [Header("Medium Obstacle Camera Shake")]
    [SerializeField] private float m_MediumDuration;
    [Range(0.00f, 1.00f)] [SerializeField] private float m_MediumIntensity;

    [Header("Heavy Obstacle Camera Shake")]
    [SerializeField] private float m_HeavyDuration;
    [Range(0.00f, 1.00f)] [SerializeField] private float m_HeavyIntensity;

    private bool m_CanCollide = true;
    private float m_CurrentTime;
    // Start is called before the first frame update
    void Start()
    {
        m_CamShake = Camera.main.GetComponent<CameraShake>();

        if (m_CamShake == null)
            return;

        m_CanCollide = true;
        m_CurrentTime = m_CollisionCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_CanCollide)
        {
            if (m_CurrentTime > 0)
            {
                m_CurrentTime -= Time.deltaTime;
            }

            else
            {
                m_CurrentTime = m_CollisionCooldown;
                m_CanCollide = true;
            }
        }
    }

    public void ExecuteCollisionShit(ObstacleTag obstacleType)
    {
        switch (obstacleType)
        {
            case ObstacleTag.None:
                break;
            case ObstacleTag.Pedestrian:
                m_CamShake.DoCameraShake(m_LightIntensity, m_LightDuration);
                break;
            case ObstacleTag.CarAI:
                m_CamShake.DoCameraShake(m_HeavyIntensity, m_HeavyDuration);
                break;
            case ObstacleTag.Light:
                m_CamShake.DoCameraShake(m_LightIntensity, m_LightDuration);
                break;
            case ObstacleTag.Medium:
                m_CamShake.DoCameraShake(m_MediumIntensity, m_MediumDuration);
                break;
            case ObstacleTag.Heavy:
                m_CamShake.DoCameraShake(m_HeavyIntensity, m_HeavyDuration);
                break;
            default:
                break;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    ObstacleType obs = collision.gameObject.GetComponent<ObstacleType>();
    //
    //    if(obs == null) return;
    //     
    //    ObstacleTag m_ObstacleType = obs.obstacleTag;
    //    if (m_CanCollide)
    //    {
    //        //Debug.Log("Collided with " + m_ObstacleType);
    //        m_CanCollide = false;
    //        ExecuteCollisionShit(m_ObstacleType);
    //    } 
    //}
}
