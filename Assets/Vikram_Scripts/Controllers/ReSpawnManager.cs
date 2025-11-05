using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReSpawnManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public BikeDataSO bikeData;
    public InputDataSO inputData;
    public UIDataSO uiData;
    public SocketDataSO socketData;

    [Header("Game Objects")]
    public GameManager gameManager;
    public GameObject HeroBike;

    [Header("Respawn Points")]
    public Transform startPoint;
    public Transform bsdPoint;
    public Transform fcwPoint;
    public Transform rcwPoint;

    private void OnEnable()
    {
        socketData.ReSpawnEvent += Respawn;
    }

    private void OnDisable()
    {
        socketData.ReSpawnEvent -= Respawn;
    }

    /// <summary>
    /// Public entry to perform a respawn. Selects one of the completed checkpoints (or start if none)
    /// If more than one checkpoint is completed, the nearest completed point to the hero bike is chosen.
    /// </summary>
    public void Respawn()
    {
        bikeData.ResetSpeed();
        inputData.DeactivateInput();

        // Build list of available respawn points based on completed features
        List<Transform> available = GetAvailableRespawnPoints();

        // If none available, fallback to startPoint
        Transform chosen = (available.Count > 0) ? ChooseRespawnPoint(available) : startPoint;

        StartCoroutine(WaitAndReSpawn(chosen));
    }

    /// <summary>
    /// Returns a list of completed respawn transforms (blindspot, FCW, RCW) in no particular order.
    /// </summary>
    private List<Transform> GetAvailableRespawnPoints()
    {
        var list = new List<Transform>();

        try
        {
            if (gameManager != null)
            {
                if (gameManager.isBlindspotCompleted && bsdPoint != null)
                    list.Add(bsdPoint);
                if (gameManager.isFCWCompleted && fcwPoint != null)
                    list.Add(fcwPoint);
                if (gameManager.isRCWCompleted && rcwPoint != null)
                    list.Add(rcwPoint);
            }
            else
            {
                Debug.LogWarning("[ReSpawnManager] gameManager not assigned — using startPoint as fallback.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[ReSpawnManager] Error while collecting available respawn points: {ex.Message}");
        }

        return list;
    }

    /// <summary>
    /// Choose a respawn point from the available list.
    /// Strategy: if more than one, pick the one nearest to the HeroBike; otherwise pick the single available one.
    /// Falls back to startPoint when HeroBike is not assigned.
    /// </summary>
    private Transform ChooseRespawnPoint(List<Transform> available)
    {
        if (available == null || available.Count == 0)
            return startPoint;

        if (available.Count == 1)
            return available[0];

        // If HeroBike is available, pick nearest respawn point to the bike
        if (HeroBike != null)
        {
            Transform nearest = null;
            float bestSqr = float.MaxValue;
            Vector3 bikePos = HeroBike.transform.position;

            foreach (var t in available)
            {
                if (t == null) continue;
                float sqr = (t.position - bikePos).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    nearest = t;
                }
            }

            if (nearest != null)
                return nearest;
        }

        // Fallback: if no HeroBike or nearest not found, pick random
        int idx = UnityEngine.Random.Range(0, available.Count);
        return available[idx] ?? startPoint;
    }

    private IEnumerator WaitAndReSpawn(Transform reSpawnPoint)
    {
        if (reSpawnPoint == null)
            reSpawnPoint = startPoint;

        uiData.FadeCanvas(1);
        yield return new WaitForSeconds(1f);

        // Place hero bike at respawn point and reset orientation/state
        HeroBike.transform.position = reSpawnPoint.position;
        HeroBike.transform.rotation = reSpawnPoint.rotation;

        // ensure inputs and speed reset after fade
        uiData.FadeCanvas(0);
        yield return new WaitForSeconds(1f);

        inputData.ActivateInput();
    }
}

[Serializable]
public class ZoneProperties
{
    public string Name;
    public GameObject SpawnPoint;
    public bool isCrossed;
}
