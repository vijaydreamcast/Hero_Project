using System;
using UnityEngine;

public class AllEnums : MonoBehaviour
{

}

[Serializable]
public enum TramMovementDirection { Forward, Backward, Left, Right }

[Serializable]
public enum TrafficLightType { East, West, North, South }

[Serializable]
public enum LightState { Green,Yellow,Red}


[Serializable]
public enum LightDirection { Front,Back}

[Serializable]
public enum FeatureType { BlindSpot,FrontVehicle,RearVehicle,CloseVehicle}


[Serializable]
public enum CollisionType { Front, Rear, Left, Right }


[Serializable]
public enum FeatureResult { Correct,Wrong}


