using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcadeVehicleController
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Vehicle m_Vehicle;

        private void Start()
        {
            Application.targetFrameRate = 60;
        }

        private void Update()
        {

            if (m_Vehicle == null) return;

            m_Vehicle.SetSteerInput(Input.GetAxis("Horizontal"));

            if (Input.GetKey(KeyCode.Space))
            {
                m_Vehicle.Braking();
                m_Vehicle.SetAccelerateInput(0.0f); // Stop acceleration while braking
            }
            else
            {
                m_Vehicle.SetAccelerateInput(Input.GetAxis("Vertical"));
            }
        }
    }
}