using UnityEngine;

public class CloseVehicleSensor : MonoBehaviour
{
    public SensorDataSO sensorData;
    public LayerMask layerMask;


    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter by "+other.gameObject.tag);
        if(other.gameObject.tag == "Car" )
        {
            Debug.Log("Event Fired");
            sensorData.CloseVehicleTriggerEnterEvent?.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car" )
        {
            sensorData.CloseVehicleTriggerExitEvent?.Invoke();
        }
    }
}
