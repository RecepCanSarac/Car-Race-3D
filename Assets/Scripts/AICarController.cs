using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;
    private Rigidbody rb;

    // AI-related fields
    [SerializeField] private Transform[] waypoints;
    private int currentWaypointIndex = 0;

    // Settings
    [SerializeField] private float motorForce, breakForce, maxSteerAngle, maxSpeed;
    [SerializeField] private float sensorLength = 10f;

    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
    }
    private void Update()
    {

        HandleAI();
    }
    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void HandleAI()
    {
        // Hedef waypoint'e doðru yön
        Vector3 directionToWaypoint = waypoints[currentWaypointIndex].position - transform.position;
        float distanceToWaypoint = directionToWaypoint.magnitude;

        // Mesafe ve waypoint güncelleme
        if (distanceToWaypoint < 1f)
        {
            // Waypoint dizisinde bir sonraki waypoint'e geçiþ yap
            currentWaypointIndex++;

            // Dizinin sonuna geldiðimizde baþa dön
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }

            Debug.Log($"Next Waypoint Index: {currentWaypointIndex}");
        }

        // Hedef waypoint'in yerel koordinatlarý
        Vector3 localTarget = transform.InverseTransformPoint(waypoints[currentWaypointIndex].position);
        horizontalInput = (localTarget.x / localTarget.magnitude);

        // Yön ve hýz ayarlarý
        float steerFactor = Mathf.Abs(horizontalInput);
        verticalInput = Mathf.Lerp(1f, 0.5f, steerFactor);

        // Engel kaçýnma
        AvoidObstacles();

        Debug.Log($"Current Waypoint Index: {currentWaypointIndex}");
        Debug.Log($"Distance to Waypoint: {distanceToWaypoint}");
    }


    private void AvoidObstacles()
    {
        RaycastHit hit;
        Vector3 sensorStartPos = transform.position;
        sensorStartPos += transform.forward * 2;
        sensorStartPos.y += 1;

        bool obstacleDetected = false;

        if (Physics.Raycast(sensorStartPos, transform.forward, out hit, sensorLength))
        {
            if (hit.collider.CompareTag("Car") || hit.collider.CompareTag("Obstacle"))
            {
                obstacleDetected = true;
                horizontalInput = (hit.normal.x > 0) ? 1 : -1;
            }
        }

        if (obstacleDetected)
        {
            verticalInput = Mathf.Clamp(verticalInput - Time.deltaTime, 0f, 1f);
        }
        else
        {
            verticalInput = Mathf.Lerp(verticalInput, 1f, Time.deltaTime);
        }
    }


    private void HandleMotor()
    {
        float speedFactor = Mathf.Clamp01(GetCurrentSpeed() / maxSpeed);
        float targetSpeedFactor = Mathf.Lerp(1f, 0.5f, speedFactor);

        float speed = verticalInput * motorForce * targetSpeedFactor;
        frontLeftWheelCollider.motorTorque = speed;
        frontRightWheelCollider.motorTorque = speed;

        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        if (Mathf.Abs(horizontalInput) > 0.8f)
        {
            currentbreakForce = breakForce;
        }

        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        float speedFactor = Mathf.Clamp01(GetCurrentSpeed() / maxSpeed);
        float steerLimit = Mathf.Lerp(maxSteerAngle, maxSteerAngle * 0.5f, speedFactor);

        currentSteerAngle = steerLimit * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private float GetCurrentSpeed()
    {
        return rb.velocity.magnitude * 3.6f;
    }

    private void AdjustWheelColliderSettings()
    {
        JointSpring suspensionSpring = frontLeftWheelCollider.suspensionSpring;
        suspensionSpring.spring = 10000;
        suspensionSpring.damper = 500;
        frontLeftWheelCollider.suspensionSpring = suspensionSpring;
        frontRightWheelCollider.suspensionSpring = suspensionSpring;

        WheelFrictionCurve forwardFriction = frontLeftWheelCollider.forwardFriction;
        forwardFriction.stiffness = 1.5f;
        frontLeftWheelCollider.forwardFriction = forwardFriction;
        frontRightWheelCollider.forwardFriction = forwardFriction;

        JointSpring rearSuspensionSpring = rearLeftWheelCollider.suspensionSpring;
        rearSuspensionSpring.spring = 10000;
        rearSuspensionSpring.damper = 500;
        rearLeftWheelCollider.suspensionSpring = rearSuspensionSpring;
        rearRightWheelCollider.suspensionSpring = rearSuspensionSpring;

        WheelFrictionCurve rearForwardFriction = rearLeftWheelCollider.forwardFriction;
        rearForwardFriction.stiffness = 1.5f;
        rearLeftWheelCollider.forwardFriction = rearForwardFriction;
        rearRightWheelCollider.forwardFriction = rearForwardFriction;
    }
}
