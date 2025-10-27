using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;


public class Tram : MonoBehaviour
{
    [Header("Vehicle Properties")]
    public Transform frontTransform;
    public SplineContainer splineContainer;
    public MovementDirection movementDirection;
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

    [Header("Local variables")]
   
    public int forwardDirection = 1; // 1 or -1
    public int upDirection = 1; // 1 or -1
    public bool isMoving = false;
    public float progress = 0;
    public float currentSpeed = 0.01f;


    private void OnDrawGizmosSelected()
    {
      
        if (frontTransform == null) return;
        Gizmos.matrix = Matrix4x4.TRS(frontTransform.position, frontTransform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.forward * maxRayCastDistance / 2,
            new Vector3(vehicleHalfLength * 2, vehicleHalfHeight * 2, maxRayCastDistance));
    }

    public void Start()
    {
  
        if (movementDirection == MovementDirection.ClockWise)
            progress = 0f;
        else
            progress = 1f;

        isMoving = true;

        Matrix4x4 local = splineContainer.transform.localToWorldMatrix;
        splineLength = splineContainer.Spline.CalculateLength(local);
    }

    private void Update()
    {
        if (!isMoving) return;
        if ( splineContainer == null) return;


            if (currentSpeed == 0f)
                currentSpeed = minSpeed;

            currentSpeed += acceleration * Time.fixedDeltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);
        

        float deltaDistance = currentSpeed * Time.fixedDeltaTime;
        float deltaProgress = (splineLength > 0f) ? (deltaDistance / splineLength) : 0f;

        if (movementDirection == MovementDirection.ClockWise)
        {
            progress = Mathf.Clamp01(progress + deltaProgress);
            SplineUtility.Evaluate(splineContainer.Spline, progress, out float3 pos, out float3 tangent, out float3 up);

            Vector3 worldPos = splineContainer.transform.TransformPoint(pos);
            Vector3 worldTangent = splineContainer.transform.TransformDirection(tangent);
            Vector3 worldUp = splineContainer.transform.TransformDirection(up);

            transform.SetPositionAndRotation(worldPos, Quaternion.LookRotation(forwardDirection * worldTangent, upDirection * worldUp));

            if (progress >= 1f)
            {
                isMoving = false;
                currentSpeed = 0f;
               
            }
        }
        else
        {
            progress = Mathf.Clamp01(progress - deltaProgress);
            SplineUtility.Evaluate(splineContainer.Spline, progress, out float3 pos, out float3 tangent, out float3 up);

            Vector3 worldPos = splineContainer.transform.TransformPoint(pos);
            Vector3 worldTangent = splineContainer.transform.TransformDirection(tangent);
            Vector3 worldUp = splineContainer.transform.TransformDirection(up);

            transform.SetPositionAndRotation(worldPos, Quaternion.LookRotation(-forwardDirection * worldTangent, upDirection * worldUp));

            if (progress <= 0f)
            {
                isMoving = false;
                currentSpeed = 0f;
               
            }
        }
    }
}