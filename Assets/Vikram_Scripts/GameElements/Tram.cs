using System;
using UnityEngine;

public class Tram : MonoBehaviour
{
    [Header("Tram Settings")]
    public TramMovementDirection direction;
    public float speed = 5f;
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    public bool isMoving = false;

    private Vector3 targetPosition;
    private Quaternion targetRotation;


    private void Update()
    {
        if(isMoving)
        {

            if(direction == TramMovementDirection.Forward)
            {
                transform.position += new Vector3(0,0,speed * Time.deltaTime);
            }
            else if (direction == TramMovementDirection.Backward)
            {
                transform.position -= new Vector3(0, 0, speed * Time.deltaTime);
            }
            else if (direction == TramMovementDirection.Right)
            {
                transform.position += new Vector3( speed * Time.deltaTime,0,0);
            }
            else if (direction == TramMovementDirection.Forward)
            {
                transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
            }

        }
    }


}


