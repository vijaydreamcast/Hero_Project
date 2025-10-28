using System.Collections.Generic;
using UnityEngine;

public class VehicleWheelRotation : MonoBehaviour
{
    [Header("Wheels")]
    public List<Transform> wheels;

    [Header("Rotation variables")]
    public float currentSpeed = 0f; // in meters per second
    public float wheelRadius = 0.3f; // in meters

    // This stores a normalized axis to rotate around (use X/Y/Z to set it in inspector)
    public float X = 1;
    public float Y = 0;
    public float Z = 0;

    // Exposed for debugging / UI, kept wrapped so it won't grow without bound
    [Tooltip("Cumulative wheel rotation (degrees), kept within -180..180 for stability")]
    public float rotationAngle = 0f;

    public MovementDirection movementDirection;
    public float speedFactor = 2f;

    // cached initial local rotations (keeps other local rotations / offsets intact)
    private List<Quaternion> initialLocalRotations;

    void Start()
    {
        currentSpeed = 0f;
        speedFactor = 60;
        // Cache initial local rotations to avoid overwriting existing orientation offsets
        if (wheels == null)
            wheels = new List<Transform>();

        initialLocalRotations = new List<Quaternion>(wheels.Count);
        foreach (var w in wheels)
            initialLocalRotations.Add(w != null ? w.localRotation : Quaternion.identity);
    }

    void Update()
    {
        for (int i = 0; i < wheels.Count; i++)
        {
            var wheel = wheels[i];
            if (wheel == null)
                continue;

            if(movementDirection == MovementDirection.CounterClockWise)
            {
                currentSpeed = -currentSpeed;
            }
            Vector3 rotationSpeed = new Vector3(X * currentSpeed, Y * currentSpeed, Z * currentSpeed);
            wheel.Rotate(rotationSpeed * Time.deltaTime * speedFactor);
        }
    }

    // Public helper so editor tools can advance rotation in edit mode without requiring Play mode.
    // Call with a simulated deltaTime (seconds) to rotate wheels the same way as Update.
    public void EditorTick(float deltaTime)
    {
        if (wheels == null) return;

        for (int i = 0; i < wheels.Count; i++)
        {
            var wheel = wheels[i];
            if (wheel == null)
                continue;

            if (movementDirection == MovementDirection.CounterClockWise)
            {
                currentSpeed = -currentSpeed;
            }
            Vector3 rotationSpeed = new Vector3(X * currentSpeed, Y * currentSpeed, Z * currentSpeed);
            wheel.Rotate(rotationSpeed * deltaTime * speedFactor, Space.Self);
        }
    }
}