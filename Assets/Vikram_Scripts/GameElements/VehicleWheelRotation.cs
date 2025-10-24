using System.Collections.Generic;
using UnityEngine;

public class VehicleWheelRotation : MonoBehaviour
{
    [Header("Wheels")]
    public float currentSpeed = 0f; // in meters per second
    public float wheelRadius = 0.3f; // in meters
    public List<Transform> wheels;
    public float rotationAngle = 0;



    void Update()
    {
        // Calculate the rotation angle based on speed and wheel radius
        rotationAngle += (currentSpeed / wheelRadius) * Mathf.Rad2Deg * Time.deltaTime;
        // Rotate each wheel around its local X-axis
        foreach (Transform wheel in wheels)
        {
            wheel.transform.localEulerAngles = new Vector3(rotationAngle, 0, 0);
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
