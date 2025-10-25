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

    // cached initial local rotations (keeps other local rotations / offsets intact)
    private List<Quaternion> initialLocalRotations;

    void Start()
    {
        // Cache initial local rotations to avoid overwriting existing orientation offsets
        if (wheels == null)
            wheels = new List<Transform>();

        initialLocalRotations = new List<Quaternion>(wheels.Count);
        foreach (var w in wheels)
            initialLocalRotations.Add(w != null ? w.localRotation : Quaternion.identity);
    }

    void Update()
    {
        if (wheels == null || wheels.Count == 0)
            return;

        // Protect against invalid radius
        if (wheelRadius <= 0f)
            return;

        // Compute incremental rotation (degrees) for this frame
        float deltaAngle = (currentSpeed / wheelRadius) * Mathf.Rad2Deg * Time.deltaTime;

        // Direction
        if (movementDirection == MovementDirection.CounterClockWise)
            deltaAngle = -deltaAngle;

        // Build rotation axis from inspector values
        Vector3 axis = new Vector3(X, Y, Z);
        if (axis.sqrMagnitude < 1e-6f)
            axis = Vector3.right; // fallback axis
        else
            axis.Normalize();

        // Apply incremental rotation to each wheel using quaternions (smooth, avoids Euler gimbal issues)
        for (int i = 0; i < wheels.Count; i++)
        {
            var wheel = wheels[i];
            if (wheel == null)
                continue;

            // Multiply by incremental rotation so any existing offset from initial rotation is preserved
            wheel.localRotation *= Quaternion.AngleAxis(deltaAngle, axis);
        }

        // Keep a bounded cumulative angle for display / logic. Use Repeat and remap to [-180,180].
        rotationAngle += deltaAngle;
        rotationAngle = Mathf.Repeat(rotationAngle + 180f, 360f) - 180f;
    }
}
