using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusControllerz : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody carRB;
    [SerializeField] private Transform[] rayPoints;
    [SerializeField] private LayerMask drivable;
    [SerializeField] private Transform accelerationPoint;

    [Header("Suspension Settings")]
    [SerializeField] private float springStiffness;
    [SerializeField] private float damperStiffness;
    [SerializeField] private float restLength;
    [SerializeField] private float springTravel;
    [SerializeField] private float wheelRadius;

    private int[] wheelIsGrounded = new int[4];
    private bool isGrounded = false;

    [Header("Input")]
    private float moveInput;
    private float steerInput;

    [Header("Car Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragCoefficient;

    private Vector3 currentCarLocalVelocity = Vector3.zero;
    private float carVelocityRatio = 0;

    // Start is called before the first frame update
    void Start()
    {
        carRB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        GetPlayerInput();
    }

    void FixedUpdate()
    {
        Suspension();
        GroundCheck();
        CalculateCarVelocity();
        Movement();
    }

    private void Suspension()
    {
        for (int i = 0; i < rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLength = restLength + springTravel;

            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLength + wheelRadius, drivable))
            {
                wheelIsGrounded[i] = 1;

                float currentSpringLength = hit.distance - wheelRadius;
                float springCompression = (restLength - currentSpringLength) / springTravel;

                float springVelocity = Vector3.Dot(carRB.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = damperStiffness * springVelocity;

                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                carRB.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);

                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }
            else
            {
                wheelIsGrounded[i] = 0;
                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position + (wheelRadius + maxLength) * -rayPoints[i].up, Color.green);
            }
        }
    }

    private void GroundCheck()
    {
        int tempGroundedWheels = 0;

        for (int i = 0; i < wheelIsGrounded.Length; i++)
        {
            tempGroundedWheels += wheelIsGrounded[i];
        }

        if (tempGroundedWheels > 1)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void GetPlayerInput()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    private void Movement()
    {
        if (isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();
        }
    }

    private void Turn()
    {
        carRB.AddTorque(steerStrength * steerInput * turningCurve.Evaluate(carVelocityRatio) * Mathf.Sin(carVelocityRatio) * transform.up, ForceMode.Acceleration);
    }

    private void SidewaysDrag()
    {
        float currentSidewaysSpeed = currentCarLocalVelocity.x;
        float dragMagnitude = -currentSidewaysSpeed * dragCoefficient;
        Vector3 dragForce = transform.right * dragMagnitude;
        carRB.AddForceAtPosition(dragForce, carRB.worldCenterOfMass, ForceMode.Acceleration);
    }

    private void Acceleration()
    {
        carRB.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Deceleration()
    {
        carRB.AddForceAtPosition(deceleration * moveInput * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void CalculateCarVelocity()
    {
        currentCarLocalVelocity = transform.InverseTransformDirection(carRB.velocity);
        carVelocityRatio = currentCarLocalVelocity.z / maxSpeed;
    }
}
