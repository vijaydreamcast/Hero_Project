using UnityEngine;

public class FrontCollisionSensor : MonoBehaviour
{
    public SensorDataSO sensorData;
    public string tagObject;


    public void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.tag == tagObject )
        {        
            sensorData.FrontCollisionTriggerEnterEvent?.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == tagObject )
        {
            sensorData.FrontCollisionTriggerExitEvent?.Invoke();
        }
    }
}
