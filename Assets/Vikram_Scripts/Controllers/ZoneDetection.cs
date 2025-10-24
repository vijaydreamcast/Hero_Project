using UnityEngine;

public class ZoneDetection : MonoBehaviour
{

    [Header("Scriptable Objects")]
    public SensorDataSO sensorData;
    public BikeDataSO bikeData;
    public InputDataSO inputData;
    public UIDataSO uiData;


    [Header("Feature Zones")]
    public GameObject BlindSpotDetectionZone;
    public GameObject FrontCollisionDetectionZone;
    public GameObject RearCollisionDetectionZone;



    public void OnTriggerEnter(Collider zone)
    {
        //Debug.Log(" collision with " + zone.gameObject.name);
        if (zone.gameObject.name == BlindSpotDetectionZone.name)
        {
            Debug.Log("Blind spot zone Entered");
            inputData.DeactivateInput();
            bikeData.ResetSpeed();           
            bikeData.BlindSpotZoneEntered();
        }
        else if (zone.gameObject.name == FrontCollisionDetectionZone.name)
        {
            Debug.Log("Front collision zone Entered");
            inputData.DeactivateInput();
            bikeData.ResetSpeed();
           
            bikeData.FrontCollisionZoneEntered();
        }
        else if (zone.gameObject.name == RearCollisionDetectionZone.name)
        {
            Debug.Log("Rear Collision zone Entered");
            inputData.DeactivateInput();
            bikeData.ResetSpeed();
           
            bikeData.RearCollisionZoneEntered();
        }



        if(zone.gameObject.tag == "Zone")
        {
            Debug.Log("Traffic zone Entered: " + zone.gameObject.GetInstanceID());
            bikeData.TraffifZoneEntered(zone.gameObject.name);
        }

        if(zone.gameObject.tag == "OutOfZone")
        {
            bikeData.OutsideZoneEntered(zone.gameObject.name);
        }
    }


    public void OnTriggerExit(Collider zone)
    {

        if (zone.gameObject.name == BlindSpotDetectionZone.name)
        {
            
            bikeData.BlindSpotZoneExited();
            sensorData.DeActivateAllSensors();
            inputData.ActivateInput();
           
        }
        else if (zone.gameObject.name == FrontCollisionDetectionZone.name)
        {
          
            bikeData.FrontCollisionZoneExited();
            sensorData.DeActivateAllSensors();
            inputData.ActivateInput();
 
        }
        else if (zone.gameObject.name == RearCollisionDetectionZone.name)
        {
           
            bikeData.RearCollisionZoneExited();
            sensorData.DeActivateAllSensors();
            inputData.ActivateInput();
      
        }

    }
}
