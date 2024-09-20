using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BusStopWaypoint : MonoBehaviour
{
    [SerializeField] private RectTransform m_WaypointPrefab;    //waypoint prefab to spawn
    [SerializeField] private float m_MinDistance;               //Min Distance from Bus Stop before it's deactivated
    [SerializeField] private Vector3 m_Offset;                  //Waypoint offset

    private RectTransform m_Waypoint;                           //The prefab instantiated associated with the Bus Stop

    private GameObject m_Bus;                                   //player gameobject

    private float m_Distance;                                   //distance between player and waypoint
    private TMP_Text m_DistanceText;                            //distance text box

    private bool m_WaypointOutside;                             //Check if the waypoint is outside of bounds
    private Image m_Pointer;                                    //The arrow image (yea idk how to rotate it tho)
    private GameObject m_WaypointHolder;                        //triangular waypoint 

    // Start is called before the first frame update
    void Start()
    {
        //initializing stuff
        var canvas = GameObject.Find("WaypointCanvas").transform;

        m_Waypoint = Instantiate(m_WaypointPrefab, canvas);

        m_Bus = GameObject.FindGameObjectWithTag("Player");

        m_DistanceText = m_Waypoint.GetComponentInChildren<TMP_Text>();

        m_Pointer = m_Waypoint.Find("Pointer").GetComponent<Image>();

        m_WaypointHolder = m_Waypoint.Find("WaypointHolder").gameObject;

        if (m_Bus == null)
            return;

        if (canvas == null)
            return;

    }

    // Update is called once per frame
    void Update()
    {
        BusStopSpawn busStopChecker = gameObject.GetComponentInChildren<BusStopSpawn>();

        if(busStopChecker != null)
        {
            //checks if there's people to collect
            if (busStopChecker.CheckPassengers() <= 0)
            {
                m_Pointer.gameObject.SetActive(false);
                m_WaypointHolder.gameObject.SetActive(false);
            }
            else
            {
                MoveWaypoint();
            }

        }
        else
        {
            MoveWaypoint();
        }
        
        
    }

    void MoveWaypoint()
    {
        //clamping of the borders of the screen
        float minX = m_Pointer.GetPixelAdjustedRect().width / 2.0f;
        float maxX = Screen.width - minX;

        float minY = m_Pointer.GetPixelAdjustedRect().height / 2.0f;
        float maxY = Screen.height - minY;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + m_Offset);

        if (screenPos.z < 0f)
        {
            // Flip the position when behind the camera
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;

            screenPos.y = minY;

            m_WaypointOutside = true;
        }
        else
        {
            m_WaypointOutside = false;
        }

        screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
        screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

        m_Waypoint.position = screenPos;

        m_Distance = Vector3.Distance(m_Bus.transform.position, transform.position);

        if (m_Distance > 500)
        {
            m_Pointer.gameObject.SetActive(m_WaypointOutside);
            m_WaypointHolder.SetActive(!m_WaypointOutside);
        }
        else
        {
            m_Pointer.gameObject.SetActive(false);
            m_WaypointHolder.SetActive(true);
        }

        if (m_Distance < m_MinDistance)
        {
            m_Waypoint.gameObject.SetActive(false);
        }
        else
        {
            m_Waypoint.gameObject.SetActive(true);
        }

        m_DistanceText.text = m_Distance.ToString("0") + " m";
    }

}
