using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class BusMovement : MonoBehaviour, IVehicleMovement
{
    [Header("Vehicle Properties")]
    public Transform frontTransform;
    public SplineContainer splineContainer;
    public float splineLength;


    [Header("Speed Settings")]
    public float maxSpeed = 5f;
    public float minSpeed = 0f;
    public float acceleration = 2f;      // m/s²
    public float deceleration = 3f;      // m/s²
    public float toleranceDistance = 0.3f;

    [Header("Detection Settings")]
    public float vehicleHalfLength = 0.5f;
    public float vehicleHalfBreadth = 0.5f;
    public float vehicleHalfHeight = 0.5f;
    public float maxRayCastDistance = 3f;
    public LayerMask obstacleMask;


    [Header(" Local variables ")]
    public MovementDirection movementDirection;
    public int forwardDirection = 1; // 1 or -1
    public int upDirection = -1; // 1 or -1
    public bool isMoving = false;
    public float progress = 0;
    public float currentSpeed = 0.01f;
    private bool obstacleAhead = false;
    private VehicleManager vm;
   




    // For readability in scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = obstacleAhead ? Color.red : Color.green;
        Gizmos.matrix = Matrix4x4.TRS(frontTransform.position, frontTransform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.forward * maxRayCastDistance / 2,
            new Vector3(vehicleHalfLength * 2, vehicleHalfHeight * 2, maxRayCastDistance));
    }

    public void StartMovement(VehicleManager vm,SplineContainer splineContainer,MovementDirection direction)
    {
        if(direction == MovementDirection.ClockWise)
        {
            progress = 0f;
        }
        else
        {
            progress = 1f;
        }
        this.vm = vm;
        this.splineContainer = splineContainer;
        movementDirection = direction;
        isMoving = true;
        Matrix4x4 local = splineContainer.transform.localToWorldMatrix;
        splineLength = splineContainer.Spline.CalculateLength(local);

    }

    void Update()
    {
        if (!isMoving) return;

        float distanceToObstacle = maxRayCastDistance;
        bool hitObstacle = Physics.BoxCast(
            frontTransform.position,
            new Vector3(vehicleHalfLength, vehicleHalfBreadth, vehicleHalfHeight),
            frontTransform.forward,
            out RaycastHit hitInfo,
            frontTransform.rotation,
            maxRayCastDistance,
            obstacleMask

        );

        if (hitObstacle)
        {
            // Measure distance to obstacle surface
            distanceToObstacle = hitInfo.distance ;

            // Predictive braking: compute required deceleration
            if ((distanceToObstacle > toleranceDistance))
            {
                currentSpeed -= deceleration * Time.fixedDeltaTime;
                if (currentSpeed < 0)
                {
                    currentSpeed = 0;
                }
            }
            else
            {              
                currentSpeed = 0f;              
            }
        }
        else
        {
            if (currentSpeed == 0f)
            {
                currentSpeed = minSpeed;
            }

            // Clear road → accelerate normally
            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

        }

        // 2) Update normalized progress using distance integration (no SplineAnimate)
        float deltaDistance = currentSpeed * Time.fixedDeltaTime; // meters covered this step
        float deltaProgress = (splineLength > 0f) ? (deltaDistance / splineLength) : 0f;

        if (movementDirection == MovementDirection.ClockWise)
        {
            progress = Mathf.Clamp01(progress + deltaProgress);
            // Evaluate spline at progress and place vehicle
            SplineUtility.Evaluate(splineContainer.Spline, progress, out float3 pos, out float3 tangent, out float3 up);

            // Convert to world space
            Vector3 worldPos = splineContainer.transform.TransformPoint(pos);
            Vector3 worldTangent = splineContainer.transform.TransformDirection(tangent);
            Vector3 worldUp = splineContainer.transform.TransformDirection(up);

            // Apply to the bus
            transform.SetPositionAndRotation(worldPos, Quaternion.LookRotation(forwardDirection * worldTangent, upDirection * worldUp));


            // 3) Stop at end of spline (or optionally loop)
            if (progress >= 1f)
            {
                isMoving = false;
                currentSpeed = 0f;
                vm.DespawnVehicle(gameObject);
            }
        }
        else
        {
             progress = Mathf.Clamp01(progress - deltaProgress);
            // Evaluate spline at progress and place vehicle
            SplineUtility.Evaluate(splineContainer.Spline, progress, out float3 pos, out float3 tangent, out float3 up);
            // Convert to world space
            Vector3 worldPos = splineContainer.transform.TransformPoint(pos);
            Vector3 worldTangent = splineContainer.transform.TransformDirection(tangent);
            Vector3 worldUp = splineContainer.transform.TransformDirection(up);
            // Apply to the bus
            transform.SetPositionAndRotation(worldPos, Quaternion.LookRotation(-forwardDirection * worldTangent, upDirection * worldUp));

            // 3) Stop at end of spline (or optionally loop)
            if (progress <= 0f)
            {
                isMoving = false;
                currentSpeed = 0f;
                vm.DespawnVehicle(gameObject);
            }
        }
       


    }
}




