using CurvedUI;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using static UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics.HapticsUtility;

public class GameManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public GameDataSO gameData;
    public UIDataSO uiData;
    public InputDataSO inputData;
    public SensorDataSO sensorData;
    public BikeDataSO bikeData;
    public SocketDataSO socketData;

    [Header("Game Elements")]
    public SimpleBikeController controller;
    public GameObject HeroBike;
    public GameObject BikeStartTransform;
    public GameObject CurvedCanvas;

    [Header("Script Elements")]
    public InputManager inputManager;
    public ArduinoInitilaze arduinoInitilaze;
    public ArduinoInput arduinoInput;

    [Header("Feature Status")]
    public bool isBlindspotCompleted;
    public bool isFCWCompleted;
    public bool isRCWCompleted;



    void OnEnable()
    {
        bikeData.isRaceCompleted = false;   

        gameData.RestartGameEvent += Restart;

        sensorData.LeftBlindSpotTriggerExitEvent += BlindspotComplete;
        sensorData.RightBlindSpotTriggerExitEvent += BlindspotComplete;

        sensorData.FrontCollisionTriggerExitEvent += FCWComplete;
        sensorData.RearCollisionTriggerExitEvent += RCWComplete;

        inputData.RightUIButtonClickedEvent += CheckFeatureStatus;
    }

    void OnDisable()
    {
        gameData.RestartGameEvent -= Restart;

        sensorData.LeftBlindSpotTriggerExitEvent -= BlindspotComplete;
        sensorData.RightBlindSpotTriggerExitEvent -= BlindspotComplete;

        sensorData.FrontCollisionTriggerExitEvent -= FCWComplete;
        sensorData.RearCollisionTriggerExitEvent -= RCWComplete;

        inputData.RightUIButtonClickedEvent -= CheckFeatureStatus;
    }

    private void RCWComplete()
    {
        isRCWCompleted = true;
      
    }

    private void FCWComplete()
    {
       isFCWCompleted = true;
    
    }

    private void BlindspotComplete()
    {
        isBlindspotCompleted = true;
    
    }

    private void CheckFeatureStatus(float val)
    {
        if(isBlindspotCompleted && isFCWCompleted && isRCWCompleted )
        {        
            StartCoroutine(BikeStoppingRoutine());
        }
       
    }


    private IEnumerator BikeStoppingRoutine()
    {
        yield return new WaitForSeconds(10);

        if (!bikeData.isRaceCompleted)
        {
            PacketData packet = new PacketData();
            packet.eventCode = EventCode.ScoreUpdated;
            packet.jsonData = gameData.GetScore().ToString();
            socketData.SendDataToClient(JsonUtility.ToJson(packet));
            gameData.isGameCompleted = true;
            inputData.DeactivateInput();
            bikeData.RaceCompleted();
        }
    }
    IEnumerator Start()
    {
        gameData.SetScore(100);
        yield return new WaitForSeconds(1.0f);

        try
        {
           
            string data = File.ReadAllText(Application.streamingAssetsPath + "/Settings/setting.JSON");
            GameSetting game_setting = JsonUtility.FromJson<GameSetting>(data);
            if (game_setting.ArduinoEnable)
            {
                arduinoInitilaze.enabled = true;
                arduinoInput.enabled = true;
                inputManager.enabled = false;
            }
            else
            {
                arduinoInitilaze.enabled = false;
                arduinoInput.enabled = false;
                inputManager.enabled = true;
            }
        }
        catch(System.Exception e)
        {
            Debug.Log("Exception: " + e.Message);
            arduinoInitilaze.enabled = false;
            arduinoInput.enabled = false;
            inputManager.enabled = true;
        }

        yield return new WaitForSeconds(10.0f);
        CurvedCanvas.GetComponent<CurvedUIRaycaster>().enabled = false;
    }

    private void Restart()
    {
        StartCoroutine(WaitAndRespawn(1.0f));
    }


    private IEnumerator WaitAndRespawn(float waitTime)
    {
        uiData.FadeCanvas(1);
        yield return new WaitForSeconds(waitTime);
        HeroBike.transform.position = BikeStartTransform.transform.position;
        HeroBike.transform.rotation = BikeStartTransform.transform.rotation;
        uiData.FadeCanvas(0);

    }
}
