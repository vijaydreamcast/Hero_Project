using System.Collections;
using UnityEngine;

public class LeftBlindSpotSensor : MonoBehaviour
{
    public SensorDataSO sensorData;
    public string tagObject;
    public float duration = 3f;
    public bool canICheck = true;

    //public void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Left Blind Spot Sensor Entered"+other.gameObject.name);
    //    if (other.gameObject.tag == tagObject)
    //    {
    //        canICheck = false;
    //        sensorData.LeftBlindSpotTriggerEnterEvent?.Invoke();
    //        StartCoroutine(WaitAndChange());
    //    }
    //}

    //public void OnTriggerExit(Collider other)
    //{
    //    Debug.Log("Left Blind Spot Sensor Exited" + other.gameObject.name);
    //    if (other.gameObject.tag == tagObject )
    //    {
    //        sensorData.LeftBlindSpotTriggerExitEvent?.Invoke();
    //    }
    //}

    private IEnumerator WaitAndChange()
    {
        yield return new WaitForSeconds(duration);
        canICheck = true;
    }

    
}
