using System.Collections.Generic;
using UnityEngine;

public class VehicleWheelRotation : MonoBehaviour
{
    [Header("Wheels")]
    public List<Transform> wheels;

    [Header("Rotation variables")]
    public float currentSpeed = 0f; // in meters per second
    public float wheelRadius = 0.3f; // in meters
    public float rotationAngle = 0;
    public MovementDirection movementDirection;
    public float X = 1;
    public float Y = 0;
    public float Z = 0;



    void Update()
    {
        // Calculate the rotation angle based on speed and wheel radius
        rotationAngle += (currentSpeed / wheelRadius) * Mathf.Rad2Deg * Time.deltaTime;
        // Rotate each wheel around its local X-axis
        foreach (Transform wheel in wheels)
        {
            if (movementDirection == MovementDirection.ClockWise)
            {
                wheel.transform.localEulerAngles = new Vector3(X*rotationAngle, Y*rotationAngle, Z*rotationAngle);
            }
            else
            {
                wheel.transform.localEulerAngles = new Vector3(-X*rotationAngle, -Y*rotationAngle, -Z*rotationAngle);
            }
        }

        if (rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        else if (rotationAngle <= -360)
        {
            rotationAngle += 360;
        }
    }
}
