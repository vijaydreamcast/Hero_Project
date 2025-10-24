using System;
using UnityEngine;

[CreateAssetMenu(fileName = "UIDataSO", menuName = "ScriptableObjects/UIDataSO")]
public class UIDataSO : ScriptableObject
{

    // Variables



    //Actions
    public Action<string> BlindSpotTestCompletedEvent;
    public Action<string> FrontCollisionTestCompletedEvent;
    public Action<string> RearCollisionTestCompletedEvent;
    public Action<string> CloseVehicleTestCompletedEvent;
    public Action ClearAllTextEvent;


    public Action GameEndEvent;


    public Action<FeatureType> ShowZoneEnterPopUpEvent;
    public Action TakeActionEvent;

    public Action<int> FadeCanvasEvent;


    //Methods

    public void FadeCanvas(int endAlpha)
    {
        FadeCanvasEvent?.Invoke(endAlpha);
    }

    public void TakeAction()
    {
        TakeActionEvent?.Invoke();
    }

    public void ShowZoneEnterPopUp(FeatureType featureType)
    {
        ShowZoneEnterPopUpEvent?.Invoke(featureType);
    }

    public void EndGame()
    {
        GameEndEvent?.Invoke();
    }

    public void BlindSpotTestCompleted(string message)
    {
        BlindSpotTestCompletedEvent?.Invoke(message);
    }

    public void FrontCollisionTestCompleted(string message)
    {
        FrontCollisionTestCompletedEvent?.Invoke(message);
    }

    public void RearCollisionTestCompleted(string message)
    {
        RearCollisionTestCompletedEvent?.Invoke(message);
    }


    public void CloseVehicleTestCompleted(string message)
    {
        CloseVehicleTestCompletedEvent?.Invoke(message);
    }


    public void ClearText()
    {
        ClearAllTextEvent?.Invoke();
    }
}
