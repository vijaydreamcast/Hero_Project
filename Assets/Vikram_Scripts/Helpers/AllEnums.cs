using JetBrains.Annotations;
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

[Serializable]
public enum Gender { Male, Female }

[Serializable]
public enum City { Delhi, Milan, SaoPaulo, Manila }

[Serializable]
public enum Language { English, Italian }

[Serializable]
public enum EventCode { PlayerInfo = 1, StartGame = 2, ScoreUpdated =3,Home = 4,ReSpawn = 5}

[Serializable]
public  class PacketData
{
    public EventCode eventCode;
    public string jsonData;
}


[Serializable]
public class PlayerInfo
{
    public string playerName;
    public string playerEmail;
    public Gender selectedGender;
    public City selectedCity;
    public Language selectedLanguage;
}


