using System;
using UnityEngine;

public class SpawnRearCollisionCar : MonoBehaviour
{
    public SensorDataSO sensorData;
    public GameObject carPrefab;
    public GameObject spawnPoint;

    [Header("Car Movement Settings")]
    public float carSpeed = 5f;
    public float moveDistance = 5f;
    private GameObject car = null;



    private void SpawnCar()
    {
        if (carPrefab == null || spawnPoint == null)
            return;

        if (car == null)
        {
            GameObject car = Instantiate(carPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            car.AddComponent<RearCollisionCarMover>().Init(carSpeed, moveDistance);
        }
    }
}

public class RearCollisionCarMover : MonoBehaviour
{
    private float speed;
    private float distance;
    private Vector3 startPos;
    private bool moving = false;

    public void Init(float moveSpeed, float moveDist)
    {
        speed = moveSpeed;
        distance = moveDist;
        startPos = transform.position;
        moving = true;
    }

    private void Update()
    {
        if (!moving) return;

        // Move along local Z axis
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= distance)
        {
            moving = false;
           // Destroy(gameObject);
        }
    }
}
