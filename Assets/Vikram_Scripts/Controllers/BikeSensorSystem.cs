using System;
using UnityEngine;

public class BikeSensorSystem : MonoBehaviour
{
    [Header("Detection Settings")]
    public SensorDataSO sensorData; // Reference to the ScriptableObject
   

    [Header("All Sensors")]
    public GameObject leftBlindSpotSensor;
    public GameObject rightBlindSpotSensor;
    public GameObject frontCollisionSensor;
    public GameObject rearCollisionSensor;
    public GameObject closeVehicleSensor;




    private void OnEnable()
    {
        ResetSensors();
        sensorData.ActivateLeftBlindSpotSensorEvent += ActivateLeftBSS;
        sensorData.ActivateFrontCollisionSensorEvent += ActivateFCS;
        sensorData.ActivateRearCollisionSensorEvent += ActivateRCS;
        sensorData.ActivateCloseVehicleSensorEvent += ActivateCVS;

        sensorData.DeActivateAllSensorsEvent += ResetSensors;

    }

    private void OnDisable()
    {
        sensorData.ActivateLeftBlindSpotSensorEvent -= ActivateLeftBSS;
        sensorData.ActivateFrontCollisionSensorEvent -= ActivateFCS;
        sensorData.ActivateRearCollisionSensorEvent -= ActivateRCS;
        sensorData.ActivateCloseVehicleSensorEvent -= ActivateCVS;

        sensorData.DeActivateAllSensorsEvent -= ResetSensors;
    }

    private void ActivateCVS()
    {
        ResetSensors();
        closeVehicleSensor.SetActive(true);
    }

    private void ActivateRCS()
    {
        ResetSensors();
        rearCollisionSensor.SetActive(true);
    }

    private void ActivateFCS()
    {
        ResetSensors();
        frontCollisionSensor.SetActive(true);
    }

    private void ActivateLeftBSS()
    {
        ResetSensors();
        leftBlindSpotSensor.SetActive(true);
    }

    private void ResetSensors()
    {
        leftBlindSpotSensor.SetActive(false);
        rightBlindSpotSensor.SetActive(false);
        frontCollisionSensor.SetActive(false);
        rearCollisionSensor.SetActive(false);
        closeVehicleSensor.SetActive(false);
    }

}
