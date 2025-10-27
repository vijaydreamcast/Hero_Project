using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SocketDataSO", menuName = "ScriptableObjects/SocketDataSO")]
public class SocketDataSO : ScriptableObject
{

    // Variables
    public string serverIP;
    public int serverPort;
    public int clientPort;
    public bool isClientConnected = false;


    //Actions
    public Action<string, int> ConnectToServerEvent;
    public Action DisconnectEvent;
    public Action<int> StartServerEvent;
    public Action StopServerEvent;

    public Action<string> SendDataToServerEvent;
    public Action<string> SendDataToClientEvent;
    public Action SetServerIpEvent;

    public Action<string> ClientConnectedEvent;
    public Action<string> ClientDisConnectedEvent;


    //Methods

    public void SetServerIp(string ip)
    {
        serverIP = ip;
        SetServerIpEvent?.Invoke();
    }
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


    public void ClientConnected()
    {
        isClientConnected = true;
        ClientConnectedEvent?.Invoke("Connected");
    }

    public void ClientDisconnected()
    {
        isClientConnected = false;
        ClientDisConnectedEvent?.Invoke("Disconnected");
    }

    public void SendDataToServer(string message)
    {
        SendDataToServerEvent?.Invoke(message);
    }

    public void SendDataToClient(string message)
    {
        SendDataToClientEvent?.Invoke(message);
    }

}
