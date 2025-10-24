using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SensorDataSO", menuName = "ScriptableObjects/SensorDataSO")]
public class SensorDataSO : ScriptableObject
{

    // Variables
    [Header("Sensor Ranges")]
    public float frontRange = 10f;
    public float rearRange = 6f;
    public float blindSpotRange = 4f;
    public float blindSpotWidth = 2f;
    public float closeDetectRadius = 3f;


    //Actions

    public Action ActivateLeftBlindSpotSensorEvent;
    public Action ActivateRightBlindSpotSensorEvent;
    public Action ActivateFrontCollisionSensorEvent;
    public Action ActivateRearCollisionSensorEvent;
    public Action ActivateCloseVehicleSensorEvent;
    public Action DeActivateAllSensorsEvent;



    public Action LeftBlindSpotTriggerEnterEvent;
    public Action LeftBlindSpotTriggerExitEvent;


    public Action RightBlindSpotTriggerEnterEvent;
    public Action RightBlindSpotTriggerExitEvent;


    public Action FrontCollisionTriggerEnterEvent;
    public Action FrontCollisionTriggerExitEvent;


    public Action RearCollisionTriggerEnterEvent;
    public Action RearCollisionTriggerExitEvent;


    public Action CloseVehicleTriggerEnterEvent;
    public Action CloseVehicleTriggerExitEvent;

    //Methods

    public void ActivateLeftBlindSpotSensor()
    {
        ActivateLeftBlindSpotSensorEvent?.Invoke();
    }

    public void ActivateRightBlindSpotSensor()
    {
        ActivateRightBlindSpotSensorEvent?.Invoke();
    }

    public void ActivateFrontCollisionSensor()
    {
        ActivateFrontCollisionSensorEvent?.Invoke();
    }

    public void ActivateRearCollisionSensor()
    {
        ActivateRearCollisionSensorEvent?.Invoke();
    }

    public void ActivateCloseVehicleSensor()
    {
        ActivateCloseVehicleSensorEvent?.Invoke();
    }


    public void DeActivateAllSensors()
    {
        DeActivateAllSensorsEvent?.Invoke();
    }

}
