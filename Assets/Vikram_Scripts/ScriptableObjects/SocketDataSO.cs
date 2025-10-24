using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SocketDataSO", menuName = "ScriptableObjects/SocketDataSO")]
public class SocketDataSO : ScriptableObject
{

    // Variables
    public string serverIP;
    public int serverPort;
    public int clientPort;


    //Actions
    public Action<string, int> ConnectToServerEvent;
    public Action DisconnectEvent;
    public Action<int> StartServerEvent;
    public Action StopServerEvent;

    public Action<string> SendDataToServerEvent;


    //Methods
    public void ConnectToServer()
    {
        ConnectToServerEvent?.Invoke(serverIP, serverPort);
    }

    public void Disconnect()
    {
        DisconnectEvent?.Invoke();
    }


    public void StartServer()
    {
        StartServerEvent?.Invoke(serverPort);
    }

    public void StopServer()
    {
        StopServerEvent?.Invoke();
    }


    public void SendDataToServer(string message)
    {
        SendDataToServerEvent?.Invoke(message);
    }

}
