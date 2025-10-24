using UnityEngine;

public class LeftBlindSpotSensor : MonoBehaviour
{
    public SensorDataSO sensorData;
    public string tagObject;

    public void OnTriggerEnter(Collider other)
    {
       
        if(other.gameObject.tag == tagObject)
        {
            
            sensorData.LeftBlindSpotTriggerEnterEvent?.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == tagObject)
        {
            sensorData.LeftBlindSpotTriggerExitEvent?.Invoke();
        }
    }
}
