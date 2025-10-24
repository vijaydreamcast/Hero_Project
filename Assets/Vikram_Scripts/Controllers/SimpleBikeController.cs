using System;
using System.IO.Ports;
using UnityEngine;


public class SimpleBikeController : MonoBehaviour
{

    [Header("Scriptable Objects")]
    public UIDataSO uiData;
    public InputDataSO inputData;
    public BikeDataSO bikeData;

    [Header("Audio sources")]
    public AudioSource bikeIdleSound;
    public AudioSource bikeRacingSound;


    [Header("Visual References")]
    public Transform steering;   // Assign in Inspector
    public Transform frontWheel;
    public Transform rearWheel; // New: assign in Inspector

    [Header("Wheel Settings")]
    public float tireRadius = 0.3f; // New: set in Inspector (meters)

    [Header("Bike Physics Settings")]
    public float accelerationForce = 50f;
    public float maxSpeed = 20f;
    public float steerAngle = 30f;
    public float leanAngle = 20f;
    public float turnSpeed = 2f;
    public float dragStrength = 3f;
    public float brakeStrength = 5f;

    [Header("Bike variables")]
    public bool isConstantSpeed = false;
    public float leftBrakeInput = 0f;
    public float rightBrakeInput = 0f;
    public bool drawGizmos = true;
    public Vector3 velocity { get; private set; }
    public float currentSpeed;
    public bool isBikeStarted = false;


    // local variables
    private float steerInput;
    private float throttleInput;
    private Vector3 lastPosition;
 
    private Quaternion initialSteerLocalRot;
    private Quaternion initialFWheelLocalRot;
    private Quaternion initialRWheelLocalRot;

    private float frontWheelAngle = 0f;
    private float rearWheelAngle = 0f;
    private float distance = 0;

    // Add this field to store the smoothed steer value
    private float smoothedSteerInput = 0f;

    private void OnEnable()
    {
        inputData.ApplyThrottleInputEvent += ApplyThrottle;
        inputData.ApplySteerInputEvent += ApplySteer;
        inputData.ApplyLeftBrakeInputEvent += ApplyLeftBrake;
        inputData.ApplyRightBrakeInputEvent += ApplyRightBrake;
        bikeData.ResetSpeedEvent += Reset;
        uiData.GameEndEvent += Reset;


        bikeData.StartBikeEvent += StartBike;
        bikeData.StopBikeEvent += StopBike;
    }

    private void OnDisable()
    {
        inputData.ApplyThrottleInputEvent -= ApplyThrottle;
        inputData.ApplySteerInputEvent -= ApplySteer;
        inputData.ApplyLeftBrakeInputEvent -= ApplyLeftBrake;
        inputData.ApplyRightBrakeInputEvent -= ApplyRightBrake;
        bikeData.ResetSpeedEvent -= Reset;
        uiData.GameEndEvent -= Reset;

        bikeData.StartBikeEvent -= StartBike;
        bikeData.StopBikeEvent -= StopBike;
    }


    private void StartBike()
    {
        isBikeStarted = true;
    }

    private void StopBike()
    {
        isBikeStarted = false;
    }
    private void ApplyLeftBrake(float strength)
    {
      
        leftBrakeInput = Mathf.Clamp01(strength);
    }

    private void ApplyRightBrake(float strength)
    {
        rightBrakeInput = Mathf.Clamp01(strength);
    }

    public void Reset()
    {
        throttleInput = 0f;
        currentSpeed = 0f;
        steerInput = 0f;
      
    }

    private void ApplyThrottle(float throttle)
    {
        throttleInput = throttle;
    }

    private void ApplySteer(float steer)
    {
        steerInput = steer;
    }

    void Start()
    {
        lastPosition = transform.position;

        if (steering != null)
            initialSteerLocalRot = steering.localRotation;

        if (frontWheel != null)
            initialFWheelLocalRot = frontWheel.localRotation;

        if (rearWheel != null)
            initialRWheelLocalRot = rearWheel.localRotation;
    }

    void Update()
    {
         if (!isBikeStarted) return;

        // Smoothly interpolate steer input
        smoothedSteerInput = Mathf.Lerp(smoothedSteerInput, steerInput, Time.deltaTime * turnSpeed * 4f);

        // --- Manual Movement ---
        if (throttleInput != 0)
        {
            currentSpeed += throttleInput * accelerationForce * Time.deltaTime;
            currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed * 0.5f, maxSpeed);
        }

        if (!isConstantSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, dragStrength * Time.deltaTime);
        }

        if (leftBrakeInput > 0 || rightBrakeInput > 0)
        {
            float totalBrake = Mathf.Clamp01(leftBrakeInput * 0.5f + rightBrakeInput * 0.5f);
            float effectiveBrakeStrength = brakeStrength * Mathf.Max(1f, totalBrake);
            float brake = (totalBrake > 0f) ? brakeStrength * totalBrake : brakeStrength;
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, brake * Time.deltaTime);
        }

        bikeData.currentSpeed = currentSpeed;

        if (Mathf.Abs(smoothedSteerInput) > 0.01f && Mathf.Abs(currentSpeed) > 0.1f)
        {
            float steer = smoothedSteerInput * steerAngle * Time.deltaTime * turnSpeed * Mathf.Sign(currentSpeed);
            transform.Rotate(0, steer, 0, Space.World);
        }

        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime, Space.Self);


        if (steering != null)
        {
            steering.localRotation = initialSteerLocalRot * Quaternion.Euler(0f, smoothedSteerInput * steerAngle, 0f);
        }

        // Calculate velocity
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        // --- Wheel Rotation ---
        float speed = velocity.magnitude * Mathf.Sign(currentSpeed); // preserve direction
        float omega = (tireRadius > 0f) ? speed / tireRadius : 0f; // rad/s
        float deltaAngle = Mathf.Rad2Deg * omega * Time.deltaTime; // degrees to rotate this frame

        // Accumulate rotation for smoothness
        frontWheelAngle += deltaAngle;
        rearWheelAngle += deltaAngle;

        // Apply rotation to wheels (X axis is rolling axis)
        if (frontWheel != null)
        {
            // Combine steering (Y) and rolling (X)
            frontWheel.localRotation = initialFWheelLocalRot *
                Quaternion.Euler(0, 0, -frontWheelAngle);
        }
        if (rearWheel != null)
        {
            rearWheel.localRotation = initialRWheelLocalRot *
                Quaternion.Euler(0, 0f, -rearWheelAngle);
        }

        distance += currentSpeed * Time.deltaTime;
        bikeData.SetSpeed(speed);
        bikeData.SetDistance(distance);

        SetBikeSounds(currentSpeed);
    }


    public void SetBikeSounds(float speed)
    {
        if (speed > 1)
        {
            float volume = Normalize(speed, 0, maxSpeed);
            bikeRacingSound.volume = volume;
            bikeIdleSound.volume = 0;
        }
        else
        {
            bikeRacingSound.volume = 0;
            bikeIdleSound.volume = 0.5f;
        }
    }


    void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 3f);
    }

    public void SetConstantSpeed(bool canIMove)
    {
        isConstantSpeed = canIMove;
    }

    public void MaintainConstantSpeed(float targetSpeed)
    {
        // Clamp the target speed to allowed range
        targetSpeed = Mathf.Clamp(targetSpeed, 0, maxSpeed);

        // Set current speed directly
        currentSpeed = targetSpeed;
        bikeData.currentSpeed = currentSpeed;

        // Optionally, set throttleInput to zero to prevent further acceleration/deceleration
        throttleInput = 0f;
    }
    /// <summary>
    /// Normalizes a value between min and max to a 0-1 range.
    /// If min == max, returns 0.
    /// </summary>
    public  float Normalize(float value, float min, float max)
    {
        if (Mathf.Approximately(max, min))
            return 0f;
        return Mathf.Clamp01((value - min) / (max - min));
    }
}

