using UnityEngine;

public class RearCollisionSensor : MonoBehaviour
{
    public SensorDataSO sensorData;
    public string tagObject;


    public void OnTriggerEnter(Collider other)
    {
       
        if(other.gameObject.tag == tagObject)
        {
            Debug.Log("Event Fired");
            sensorData.RearCollisionTriggerEnterEvent?.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == tagObject )
        {
            sensorData.RearCollisionTriggerExitEvent?.Invoke();
        }
    }
}
