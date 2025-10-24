using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputDataSO", menuName = "ScriptableObjects/InputDataSO")]
public class InputDataSO : ScriptableObject
{

    // Variables
    public bool isInputActivated = false;
    public float leftbrake;
    public float rightbrake;

    //Actions
    public Action<float> ApplyThrottleInputEvent;
    public Action<float> ApplySteerInputEvent;
    public Action<float> ApplyLeftBrakeInputEvent;
    public Action<float> ApplyRightBrakeInputEvent;

    public Action<float> LeftUIButtonClickedEvent;
    public Action<float> RightUIButtonClickedEvent;


    public Action<string> HapticFeedBackEvent;



    //Methods

    private void OnEnable()
    {
        isInputActivated = false;
        leftbrake = 0f;
        rightbrake = 0f;
    }

    public void SendHapticFeedBack()
    {
        HapticFeedBackEvent?.Invoke("R");
    }

    public void ActivateInput()
    {
        isInputActivated = true;
    }

    public void DeactivateInput()
    {
        isInputActivated = false;
    }


    public void ApplyThrottle(float throttle)
    {
        ApplyThrottleInputEvent?.Invoke(throttle);
    }

    public void ApplySteer(float steer)
    {
        ApplySteerInputEvent?.Invoke(steer);
    }

    public void ApplyLeftBrake(float brake)
    {
       ApplyLeftBrakeInputEvent?.Invoke(brake);
      
    }

    public void ApplyRightBrake(float brake)
    {
        ApplyRightBrakeInputEvent?.Invoke(brake);
    }


    public void LeftUIClicked()
    {
        LeftUIButtonClickedEvent?.Invoke(1);
    }

    public void RightUIClicked()
    {
        RightUIButtonClickedEvent?.Invoke(1);
    }


 
}
