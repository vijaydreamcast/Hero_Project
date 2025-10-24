using UnityEngine;

public class MoveRearCollisionBus : MonoBehaviour
{
    public SensorDataSO sensorData;
    public GameObject bus;

    [Header("Car Movement Settings")]
    public float busSpeed = 5f;
    public float moveDistance = 10f;

    private Vector3 startPos;
    private bool isMoving = false;



    private void MoveBus()
    {
        if (bus == null)
            return;

        startPos = bus.transform.position;
        isMoving = true;
    }

    private void Update()
    {
        if (!isMoving || bus == null)
            return;

        // Move the bus along its local Z axis
        bus.transform.Translate(Vector3.forward * busSpeed * Time.deltaTime);

        // Check if the bus has moved the required distance
        float traveled = Vector3.Distance(startPos, bus.transform.position);
        if (traveled >= moveDistance)
        {
            isMoving = false;
        }
    }
}
