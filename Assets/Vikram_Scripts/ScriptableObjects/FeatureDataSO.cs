using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(fileName = "FeatureDataSO", menuName = "ScriptableObjects/FeatureDataSO")]
public class FeatureDataSO : ScriptableObject
{

    // Variables
    public FeatureData BlindSpotData;
    public FeatureData FrontCollisionData;
    public FeatureData RearCollisionData;
    public FeatureData CloseVehicleData;


    //Actions
  


    //Methods

}

[Serializable]
public class FeatureData
{
    public FeatureType featureType;
    public string featureName;
    public string featureDescription;
    public string correctAction;
    public string wrongAction;
    public string safetyMessage;
    public Sprite featureIcon;
}
