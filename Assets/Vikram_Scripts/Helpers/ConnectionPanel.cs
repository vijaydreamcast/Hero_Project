using TMPro;
using UnityEngine;

public class ConnectionPanel : MonoBehaviour
{
    [Header("Scriptable Object")]
    public SocketDataSO socketData;


    [Header("UI Elements")]
    public TMP_Text serverIpText;
    public TMP_Text connectionText;


    private void OnEnable()
    {
        socketData.SetServerIpEvent += SetIp;
        socketData.ClientConnectedEvent += SetConnectionStatus;
        socketData.ClientDisConnectedEvent += SetConnectionStatus;
    }

    private void OnDisable()
    {
        socketData.SetServerIpEvent -= SetIp;
        socketData.ClientConnectedEvent -= SetConnectionStatus;
        socketData.ClientDisConnectedEvent -= SetConnectionStatus;
    }


    public void SetIp(string ip)
    {
        serverIpText.text = "Server Ip: " + socketData.serverIP;
    }

    public void SetConnectionStatus(string status)
    {
        connectionText.text = status;
    }

}
