using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class VehicleManager : MonoBehaviour
{
    [Header(" Vehicle Prefabs")]
    public SplineContainer splineContainer;
    public List<GameObject> vehiclePrefabs;
    public List<GameObject> activeVehicles = new List<GameObject>();

    [Header(" Pool Settings")]
    public int poolSizePerPrefab = 10; // how many per prefab type
    private Dictionary<GameObject, Queue<GameObject>> vehiclePools = new Dictionary<GameObject, Queue<GameObject>>();

    [Header(" Prewarm Settings (avoid single-frame spikes)")]
    public bool prewarmPools = true;
    [Tooltip("Number of Instantiate operations per frame when prewarming")]
    public int instantiationsPerFrame = 10;

    [Header(" Spawn Settings")]
    public int maxVehicles = 10;
    public float minSpawnInterval = 3f;
    public float maxSpawnInterval = 6f;

    [Header(" Vehicle Properties")]
    public MovementDirection movementDirection;
    public float vehicleHalfLength = 1.5f;
    public float vehicleHalfBreadth = 1f;
    public float vehicleHalfHeight = 1f;
    public Transform spawnPoint;

    Vector3 halfExtents;

    // state
    private bool poolsReady = false;

    private void Start()
    {
        halfExtents = new Vector3(2 * vehicleHalfLength, vehicleHalfBreadth, vehicleHalfHeight);
        StartCoroutine(InitializePoolsRoutine());
    }

    private IEnumerator InitializePoolsRoutine()
    {
        vehiclePools.Clear();
        if (vehiclePrefabs == null || vehiclePrefabs.Count == 0)
        {
            poolsReady = true;
            StartCoroutine(SpawnVehicleRoutine());
            yield break;
        }

        int counter = 0;
        foreach (var prefab in vehiclePrefabs)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                // Instantiate inactive clone parented to this manager to keep hierarchy clean
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                pool.Enqueue(obj);

                counter++;
                if (prewarmPools && counter % Mathf.Max(1, instantiationsPerFrame) == 0)
                {
                    // yield to next frame to spread CPU and GC pressure
                    yield return null;
                }
            }
            vehiclePools[prefab] = pool;

            // small yield between types to reduce hiccups when many prefabs
            if (prewarmPools)
                yield return null;
        }

        poolsReady = true;
        // now it's safe to start spawning
        StartCoroutine(SpawnVehicleRoutine());
        yield break;
    }

    private IEnumerator SpawnVehicleRoutine()
    {
        // Wait until pools are ready (safety)
        while (!poolsReady)
            yield return null;

        while (true)
        {
            TrySpawnVehicle();
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void TrySpawnVehicle()
    {
        if (!poolsReady) return;

        if (vehiclePrefabs.Count == 0 || activeVehicles.Count >= maxVehicles || spawnPoint == null)
            return;

        Collider[] overlaps = Physics.OverlapBox(
            spawnPoint.position,
            halfExtents,
            spawnPoint.rotation,
            LayerMask.GetMask("Vehicle")
        );

        if (overlaps.Length > 0)
            return; // area blocked

        // Pick random prefab type
        GameObject prefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Count)];
        if (!vehiclePools.TryGetValue(prefab, out Queue<GameObject> pool) || pool.Count == 0)
            return; // pool empty

        GameObject vehicleInstance = pool.Dequeue();
        vehicleInstance.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
        vehicleInstance.SetActive(true);

        IVehicleMovement movement = vehicleInstance.GetComponent<IVehicleMovement>();
        movement.StartMovement(this, splineContainer, movementDirection);

        activeVehicles.Add(vehicleInstance);
    }

    public void DespawnVehicle(GameObject obj)
    {
        if (!activeVehicles.Contains(obj)) return;

        activeVehicles.Remove(obj);
        obj.SetActive(false);

        // Find which prefab type this came from (based on name or prefab match)
        foreach (var kvp in vehiclePools)
        {
            if (obj.name.StartsWith(kvp.Key.name))
            {
                kvp.Value.Enqueue(obj);
                return;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoint == null) return;
        Vector3 halfExtents = new Vector3(2 * vehicleHalfLength, vehicleHalfBreadth, vehicleHalfHeight);
        Gizmos.color = Color.green;
        Gizmos.matrix = Matrix4x4.TRS(spawnPoint.position, spawnPoint.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);
    }
}

public enum MovementDirection { ClockWise, CounterClockWise }