using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using UnityEngine.SceneManagement;

public class ArduinoInitilaze : MonoBehaviour
{
    [Header("Scriptable Object")]
    public InputDataSO inputData;

    public SerialPort stream;
    [SerializeField] string port = "COM4";
    [SerializeField] int baudRate=9600;


    public string value;
    private Thread readData;

    [SerializeField] public float arduinoAngle;
    [SerializeField] public int leftBrake;
    [SerializeField] public int rightBrake;
    [SerializeField] public float throttle;
    private void Awake()
    {
        string data = File.ReadAllText(Application.streamingAssetsPath + "/Settings/setting.JSON");
        GameSetting game_setting = JsonUtility.FromJson<GameSetting>(data);
        port = game_setting.Port;
        baudRate = game_setting.BaudRate;
    }

    private void OnEnable()
    {
        try
        {
            stream = new SerialPort("\\\\.\\" + port, baudRate);
            stream.DtrEnable = true;
            stream.RtsEnable = true;
            stream.ReadTimeout = 20000000; stream.WriteTimeout = 2000000;
            stream.Open();
            ArduinoHandle();
        }
        catch (Exception error)
        {
            Debug.Log(error);
        }

        inputData.HapticFeedBackEvent += SendMessage;
    }


    // Update is called once per frame
    void Update()
    {

        ApplicationQuitShourtcut();
        ApplicationRestartShourtcut();
    }

    public void SendMessage(string msg)
    {
        try
        {
            stream.Write(msg);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    void ArduinoHandle()
    {
        if (!stream.IsOpen)
        {
            Debug.Log("Serial Port Is Not Open");
        }
        else
        {

            StartCoroutine(ArduinoStartCommand());
            readData = new Thread(GetData);
            readData.Start();
        }
    }
    IEnumerator ArduinoStartCommand()
    {
        yield return new WaitForSeconds(2f);
        if (stream.IsOpen)
        {
            try
            {
                //stream.Write("S\n");
            }
            catch (Exception error)
            {
                Debug.Log(error);
            }
        }
    }
    void GetData()
    {
        while (stream.IsOpen)
        {

            value = stream.ReadLine();
            try
            {
                string[] arduino_vlaues = value.Split(",");
                if (arduino_vlaues.Length == 4)
                {
                    arduinoAngle = float.Parse(arduino_vlaues[0]);
                    rightBrake = int.Parse(arduino_vlaues[1]);
                    leftBrake = int.Parse(arduino_vlaues[2]);
                    throttle = float.Parse(arduino_vlaues[3]);

                    // Normalize arduinoAngle to range -1 to +1
                    float normalizeSteer = Mathf.InverseLerp(-22f, 26f, arduinoAngle) * 2f - 1f;
                    normalizeSteer = (float)Math.Round(normalizeSteer, 1);
                    float customAccelerationAxis = Mathf.InverseLerp(400f, 1023f, throttle);

                    if (inputData.isInputActivated)
                    {
                        inputData.ApplyThrottle(customAccelerationAxis);
                        inputData.ApplySteer(normalizeSteer);
                        inputData.ApplyLeftBrake(leftBrake);
                        inputData.ApplyRightBrake(rightBrake);
                    }

                    inputData.leftbrake = leftBrake;
                    inputData.rightbrake = rightBrake;                  

                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        }
    }

    private void OnDisable()
    {
        if (stream.IsOpen)
        {
            try
            {
                //stream.Write("E\n");
                readData.Abort();
                stream.Close();
            }
            catch (Exception error)
            {
                Debug.Log(error);
            }
        }
        inputData.HapticFeedBackEvent -= SendMessage;
    }

    private void OnApplicationQuit()
    {
        if (stream.IsOpen)
        {
            try
            {
                //stream.Write("E\n");
                readData.Abort();
                stream.Close();
            }
            catch (Exception error)
            {
                Debug.Log(error);
            }
        }
    }

    void ApplicationQuitShourtcut()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Application.Quit();
            }
        }
    }

    void ApplicationRestartShourtcut()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                try
                {
                    //stream.Write("E\n");
                    readData.Abort();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
                ResetGame();
            }
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }



}
