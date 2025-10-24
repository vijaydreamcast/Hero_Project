using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ReSpawnManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public BikeDataSO bikeData;
    public InputDataSO inputData;
    public UIDataSO uiData;
    public GameDataSO gameData;

    [Header("Game Objects")]
    public GameObject HeroBike;
    public List<ZoneProperties> zonePropeties;
    public List<ZoneProperties> OutOfZonePropeties;


    private void OnEnable()
    {
        bikeData.TrafficZoneEnterEvent += TrafficZoneEntered;
        bikeData.OutsideZoneEnteredEvent += OutsideEntered;
        gameData.RestartGameEvent += Restart;
    }

    private void OnDisable()
    {
        bikeData.TrafficZoneEnterEvent -= TrafficZoneEntered;
        bikeData.OutsideZoneEnteredEvent -= OutsideEntered;
        gameData.RestartGameEvent -= Restart;
    }

    private void Restart()
    {
        for (int i = 1; i < zonePropeties.Count; i++)
        {
            zonePropeties[i].isCrossed = false;
        }
    }

    private void OutsideEntered(string name)
    {
   
        for (int i = 0; i < OutOfZonePropeties.Count; i++)
        {
            var zone = OutOfZonePropeties[i];
            if (zone.Name == name)
            {
                    Debug.Log(" Out Of Zone "+name);
                    bikeData.ResetSpeed();
                    inputData.DeactivateInput();
                    StartCoroutine(WaitAndReSpawn(zone.SpawnPoint.transform));
            }
        }
    }

    private void TrafficZoneEntered(string name)
    {
        for (int i = 0; i < zonePropeties.Count; i++)
        {
            var zone = zonePropeties[i];
            if (zone.Name == name)
            {
                if (zone.isCrossed)
                {
                    Debug.Log(" Zone already crossed "+name);
               
                    // Reset isCrossed for the next zone if it exists
                    int nextIndex = i + 1;
                    if (nextIndex < zonePropeties.Count)
                    {
                        zonePropeties[nextIndex].isCrossed = false;
                    }

                    bikeData.ResetSpeed();
                    inputData.DeactivateInput();
                    StartCoroutine(WaitAndReSpawn(zone.SpawnPoint.transform));

                }
                else
                {
                    Debug.Log(" Zone  crossed first time"+name);
                    zone.isCrossed = true;
                }
            }
        }
    }

    private IEnumerator WaitAndReSpawn(Transform reSpawnPoint)
    {
        uiData.FadeCanvas(1);
        yield return new WaitForSeconds(1);
        HeroBike.transform.position = reSpawnPoint.transform.position;
        HeroBike.transform.rotation = reSpawnPoint.transform.rotation;
        uiData.FadeCanvas(0);
        yield return new WaitForSeconds(1);

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
