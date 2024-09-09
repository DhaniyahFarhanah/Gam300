using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BusController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody rb;          //bus rigidbody
    [SerializeField] Transform[] rayPoints; //raypoints for suspension
    [SerializeField] LayerMask drivable;    //layer mask for what is drivable so the 
    [SerializeField] private Transform accelerationPoint;

    [Header("Suspension Settings")]
    [SerializeField] float springStiffness; //max force he spring can exert when fully compressed
    [SerializeField] float damper;          //the amount of dampening in suspension
    [SerializeField] float restLength;      //how long the spring is normally without any forces
    [SerializeField] float springTravel;    //the max/min distance the spring can stretch and compress
    [SerializeField] float wheelRadius;     //radius of the wheel

    private int[] wheelIsGrounded = new int[4];
    private bool isGrounded = false;

    [Header("Input")]
    private float moveInput = 0;
    private float steerInput = 0;

    [Header("Car Settings")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float steerStrength = 15f;
    [SerializeField] private AnimationCurve turningCurve;
    [SerializeField] private float dragEq = 1f;

    private Vector3 currentCarVelocity = Vector3.zero;
    private float carVelocityRatio = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
    }

    private void FixedUpdate()
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

                float springVelocity = Vector3.Dot(rb.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float dampForce = damper * springVelocity;

                float springForce = springStiffness * springCompression;

                float netForce = springForce - dampForce;

                rb.AddForceAtPosition(netForce * rayPoints[i].up, rayPoints[i].position);

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
        int tempGroundWheels = 0;

        for(int i = 0; i < wheelIsGrounded.Length; i++)
        {
            tempGroundWheels += wheelIsGrounded[i];
        }

        if(tempGroundWheels > 1)
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
        if(isGrounded)
        {
            Acceleration();
            Deceleration();
            Turn();
            SidewaysDrag();
        }
    }
    private void Turn()
    {
        rb.AddRelativeTorque(steerStrength * steerInput * turningCurve.Evaluate(Mathf.Abs(carVelocityRatio)) * Mathf.Sign(carVelocityRatio) * rb.transform.up, ForceMode.Acceleration);
    }
    private void SidewaysDrag()
    {
        float currentSidewaysSpeed = currentCarVelocity.x;

        float dragMagnitude = -currentSidewaysSpeed * dragEq;

        Vector3 dragForce = transform.right * dragMagnitude;

        rb.AddForceAtPosition(dragForce, rb.worldCenterOfMass, ForceMode.Acceleration);
    }

    private void Acceleration()
    {
        rb.AddForceAtPosition(acceleration * moveInput * transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void Deceleration()
    {
        rb.AddForceAtPosition(deceleration * moveInput * -transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }

    private void CalculateCarVelocity()
    {
        currentCarVelocity = transform.InverseTransformDirection(rb.velocity);
        carVelocityRatio = currentCarVelocity.x / maxSpeed;
    }
}
