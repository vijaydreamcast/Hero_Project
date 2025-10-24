using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BikeDataSO", menuName = "ScriptableObjects/BikeDataSO")]
public class BikeDataSO : ScriptableObject
{

    // Variables

    public float currentSpeed;
    public float currentDistance;
    public float totalTime;


    //Actions
    public Action BlindSpotZoneEnterEvent;
    public Action BlindSpotZoneExitEvent;

    public Action FrontCollisionZoneEnterEvent;
    public Action FrontCollisionZoneExitEvent;

    public Action RearCollisionZoneEnterEvent;
    public Action RearCollisionZoneExitEvent;



    public Action RaceCompletedEvent;


    public Action<string> TrafficZoneEnterEvent;
    public Action<string> OutsideZoneEnteredEvent;


    public Action ResetSpeedEvent;
    public Action<GameObject> BikeCollidedEvent;
    public Action<CollisionType> BikeCollidedDirectionEvent;


    public Action StartBikeEvent;
    public Action StopBikeEvent;



    //Methods

    private void OnEnable()
    {
        currentSpeed = 0;
        currentDistance = 0;
       
    }

    public void StartBike()
    {
        StartBikeEvent?.Invoke();
    }

    public void StopBike()
    {
        StopBikeEvent?.Invoke();
    }

    public void RaceCompleted()
    {
        RaceCompletedEvent?.Invoke();
    }

    public void BlindSpotZoneEntered()
    {
        BlindSpotZoneEnterEvent?.Invoke();
    }

    public void BlindSpotZoneExited()
    {
        BlindSpotZoneExitEvent?.Invoke();
    }

    public void FrontCollisionZoneEntered()
    {
        FrontCollisionZoneEnterEvent?.Invoke();
    }

    public void FrontCollisionZoneExited()
    {
        FrontCollisionZoneExitEvent?.Invoke();
    }

    public void RearCollisionZoneEntered()
    {
        RearCollisionZoneEnterEvent?.Invoke();
    }

    public void RearCollisionZoneExited()
    {
        RearCollisionZoneExitEvent?.Invoke();
    }


    public void TraffifZoneEntered(string zoneName)
    {
        TrafficZoneEnterEvent?.Invoke(zoneName);
    }

    public void OutsideZoneEntered(string zoneName)
    {
        OutsideZoneEnteredEvent?.Invoke(zoneName);
    }

    public void ResetSpeed()
    {
        ResetSpeedEvent?.Invoke();
    }

    public void BikeCollided(GameObject obj)
    {
        BikeCollidedEvent?.Invoke(obj);
    }

    public void BikeCollidedDirection(CollisionType collisionType)
    {
        BikeCollidedDirectionEvent?.Invoke(collisionType);
    }

    public void SetSpeed(float speed)
    {
        // Convert speed from m/s to km/h
        currentSpeed = speed * 3.6f;
    }

    public void SetDistance(float distance)
    {
        currentDistance = distance / 1000;
    }

    
}
