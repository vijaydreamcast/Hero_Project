using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class BSDCarMovement : MonoBehaviour
{
    [Header("Vehicle Setup")]

    public VehicleWheelRotation WheelRotation;
    public SplineContainer splineContainer;
    public float splineLength;
    public float progress = 0f;

    [Header("Runtime Variables")]
    
    [SerializeField]private bool isMoving = false;
    public MovementDirection movementDirection = MovementDirection.ClockWise;
    public float currentSpeed = 0f; // set externally (e.g., from bike)
    public int forwardDirection = 1;
    public int upDirection = 1;



    private void Start()
    {
        StartMovement();
    }

    public void SetMovement(bool isMoving)
    {
        this.isMoving = isMoving;
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }
    public void StartMovement()
    {

    //    progress = movementDirection == MovementDirection.ClockWise ? 0f : 1f;
        isMoving = true;

        Matrix4x4 local = splineContainer.transform.localToWorldMatrix;
        splineLength = splineContainer.Spline.CalculateLength(local);
    }

    void Update()
    {
        if (!isMoving || splineContainer == null) return;

        // Calculate distance covered per frame
        float deltaDistance = currentSpeed * Time.deltaTime;
        float deltaProgress = splineLength > 0f ? deltaDistance / splineLength : 0f;

        // Move along spline depending on direction
        if (movementDirection == MovementDirection.ClockWise)
        {
            progress = Mathf.Clamp01(progress + deltaProgress);
        }
        else
        {
            progress = Mathf.Clamp01(progress - deltaProgress);
        }

        // Evaluate spline and position object
        SplineUtility.Evaluate(splineContainer.Spline, progress, out float3 pos, out float3 tangent, out float3 up);

        Vector3 worldPos = splineContainer.transform.TransformPoint(pos);
        Vector3 worldTangent = splineContainer.transform.TransformDirection(tangent);
        Vector3 worldUp = splineContainer.transform.TransformDirection(up);

        transform.SetPositionAndRotation(worldPos, Quaternion.LookRotation(forwardDirection * worldTangent, upDirection * worldUp));

        if (WheelRotation != null)
        {
            WheelRotation.currentSpeed = currentSpeed;
        }

        // Optional: stop at ends
        if ((movementDirection == MovementDirection.ClockWise && progress >= 1f) ||
            (movementDirection == MovementDirection.CounterClockWise && progress <= 0f))
        {
            
            isMoving = false;
        }
    }
}
