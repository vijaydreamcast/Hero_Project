using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// A simple UDP server that binds to a port and listens for client messages.
/// Works in Unity Editor/Build on desktop.
/// </summary>
public class ServerManager : MonoBehaviour
{
    [Header(" Scriptable Object")]
    public SocketDataSO socketData; // Your scriptable object holding IP/port config


    // local variables
    private UdpClient udpServer;
    private Thread receiveThread;
    private bool isRunning;


    public void OnEnable()
    {
        StartServer(socketData.serverPort);
    }

    public void OnDisable()
    {
        StopServer();
    }
    /// <summary>
    /// Start the server by binding to the given port.
    /// </summary>
    public void StartServer(int port)
    {
        if (isRunning) return;

        try
        {
            udpServer = new UdpClient(port); // bind to the specific port
            isRunning = true;

            receiveThread = new Thread(() => ReceiveLoop(port));
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log($"[ServerManager] Server started. Listening on port {port}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ServerManager] Failed to start server: {ex.Message}");
        }
    }

    /// <summary>
    /// Stop the server and close resources.
    /// </summary>
    public void StopServer()
    {
        isRunning = false;
        try
        {
            udpServer?.Close();
            receiveThread?.Abort();
        }
        catch { }
        Debug.Log("[ServerManager] Server stopped.");
    }

    /// <summary>
    /// Background loop to receive UDP messages.
    /// </summary>
    private void ReceiveLoop(int port)
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);

        while (isRunning)
        {
            try
            {
                byte[] data = udpServer.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);


                Debug.Log("Received " + message);

                //// Optional: Echo back to client
                //byte[] echoBytes = Encoding.UTF8.GetBytes("Echo: " + message);
                //udpServer.Send(echoBytes, echoBytes.Length, remoteEndPoint);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ServerManager] Receive failed: {ex.Message}");
            }
        }
    }


    public void BroadCastData(string data)
    {
        if (!isRunning || udpServer == null)
        {
            Debug.LogWarning("[ServerManager] Cannot broadcast, server not running.");
            return;
        }

        try
        {
            // Convert string to bytes
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            // Enable broadcast on the socket
            udpServer.EnableBroadcast = true;

            // Broadcast to all clients on the local subnet
            IPEndPoint broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, socketData.clientPort);

            udpServer.Send(bytes, bytes.Length, broadcastEndPoint);

            Debug.Log($"[ServerManager] Broadcasted: \"{data}\" to {broadcastEndPoint}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ServerManager] Broadcast failed: {ex.Message}");
        }
    }

}
