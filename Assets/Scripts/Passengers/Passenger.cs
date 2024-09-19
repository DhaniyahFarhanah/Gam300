using ArcadeVehicleController;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Passenger : MonoBehaviour
{
    public bool m_PickedUp;
    public bool m_IsPickedUp;
    public GameObject m_target;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        m_PickedUp = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target == null)
            return;
        if (m_PickedUp)
        {
            speed += Time.deltaTime * 50f;
            transform.position = Vector3.MoveTowards(transform.position, m_target.transform.position, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_PickedUp)
        {
            if (other.CompareTag("Player"))
            {
                if (!m_IsPickedUp)
                {
                    m_IsPickedUp = true;
                    other.GetComponent<Vehicle>().m_Passengers++;
                    Destroy(this.gameObject, 0.2f);
                }
                
            }
        }
    }
}
