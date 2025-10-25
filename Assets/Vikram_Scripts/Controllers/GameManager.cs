using CurvedUI;
using System.Collections;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public GameDataSO gameData;
    public UIDataSO uiData;
    public InputDataSO inputData;

    [Header("Game Elements")]
    public GameObject HeroBike;
    public GameObject BikeStartTransform;
    public GameObject CurvedCanvas;

    [Header("Script Elements")]
    public InputManager inputManager;
    public ArduinoInitilaze arduinoInitilaze;
    public ArduinoInput arduinoInput;

     IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        CurvedCanvas.GetComponent<CurvedUIRaycaster>().enabled = false;
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

    private void OnEnable()
    {
       
            gameData.RestartGameEvent += Restart;
    }

    private void OnDisable()
    {
        gameData.RestartGameEvent -= Restart;
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
