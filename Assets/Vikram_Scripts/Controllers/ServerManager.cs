using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// A simple TCP server that binds to a port and listens for client connections and messages.
/// Works in Unity Editor/Build on desktop.
/// </summary>
public class ServerManager : MonoBehaviour
{
    // Singleton instance
    public static ServerManager Instance { get; private set; }

    [Header(" Scriptable Object")]
    public SocketDataSO socketData; // Your scriptable object holding IP/port config
    public UIDataSO uiData;
    public PacketData packetData;


    // TCP server variables
    private TcpListener tcpListener;
    private Thread listenThread;
    private List<TcpClient> connectedClients = new List<TcpClient>();
    public bool isRunning;
    private bool isClientConnected;
    private bool isMessageReceived;
    

    [Header("Inspector Broadcast")]
    [Tooltip("Message to send when you click the inspector button (Play mode only)")]
    public string inspectorBroadcastMessage = "Hello from ServerManager";

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnEnable()
    {
        string localIP = GetLocalIPv4();
        Debug.Log($"[ServerManager] Local IPv4 Address: {localIP}");
        socketData.SetServerIp(localIP);
        StartServer(socketData.serverPort);

        socketData.SendDataToClientEvent += SendDataToClients;
    }

    public void OnDisable()
    {
        socketData.SendDataToClientEvent -= SendDataToClients;
        StopServer();
    }

    /// <summary>
    /// Start the TCP server by binding to the given port.
    /// </summary>
    public void StartServer(int port)
    {
        if (isRunning) return;

        try
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            isRunning = true;

            listenThread = new Thread(ListenForClients);
            listenThread.IsBackground = true;
            listenThread.Start();

            Debug.Log($"[ServerManager] TCP Server started. Listening on port {port}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ServerManager] Failed to start TCP server: {ex.Message}");
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
            tcpListener?.Stop();
            lock (connectedClients)
            {
                foreach (var client in connectedClients)
                {
                    client?.Close();
                }
                connectedClients.Clear();
            }
            listenThread?.Abort();
        }
        catch { }
        Debug.Log("[ServerManager] TCP Server stopped.");
    }

    /// <summary>
    /// Thread loop to accept incoming TCP clients.
    /// </summary>
    private void ListenForClients()
    {
        while (isRunning)
        {
            try
            {
                TcpClient client = tcpListener.AcceptTcpClient();

                // Disable Nagle's algorithm so small messages are sent immediately.
                try
                {
                    client.NoDelay = true;
                }
                catch { }

                lock (connectedClients)
                {
                    connectedClients.Add(client);
                }
                Debug.Log($"[ServerManager] Client connected: {client.Client.RemoteEndPoint}");
                isClientConnected = true;
               

                // Start a thread to handle communication with this client
                Thread clientThread = new Thread(() => HandleClientComm(client));
                clientThread.IsBackground = true;
                clientThread.Start();
            }
            catch (SocketException ex)
            {
                if (isRunning)
                    Debug.LogError($"[ServerManager] Accept failed: {ex.Message}");
            }
        }
    }

    private void Update()
    {
        if(isClientConnected)
        {
            socketData.ClientConnected();
            isClientConnected = false;
        }

        if(isMessageReceived)
        {
            isMessageReceived = false;

            if(packetData.eventCode == EventCode.PlayerInfo)
            {
              
            }

            else if(packetData.eventCode == EventCode.Home)
            {
               LoadScene(0);
            }


        }
    }

    private void LoadScene(int index)
    {
       
        SceneManager.LoadSceneAsync(index);
    }

    /// <summary>
    /// Handles communication with a connected TCP client.
    /// </summary>
    private void HandleClientComm(TcpClient client)
    {
        NetworkStream clientStream = client.GetStream();
        byte[] message = new byte[4096];
        int bytesRead;

        try
        {
            while (isRunning && client.Connected)
            {
                bytesRead = 0;
                bytesRead = clientStream.Read(message, 0, 4096);
                if (bytesRead == 0)
                {
                    // The client has disconnected
                    break;
                }

                string receivedMsg = Encoding.UTF8.GetString(message, 0, bytesRead);
              

                if(!string.IsNullOrEmpty(receivedMsg) && receivedMsg.Length > 4) // Assuming valid PlayerInfo JSON is longer than 50 characters
                {
                    Debug.Log($" Received {receivedMsg}");
                   
                    packetData = JsonUtility.FromJson<PacketData>(receivedMsg);
                    isMessageReceived = true;
                    if (packetData.eventCode == EventCode.PlayerInfo)
                    {
                        uiData.PlayerInfo = JsonUtility.FromJson<PlayerInfo>(packetData.jsonData);
                    }
                }
                
                
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ServerManager] Client communication error: {ex.Message}");
        }
        finally
        {
            Debug.Log($"[ServerManager] Client disconnected: {client.Client.RemoteEndPoint}");
           // socketData.ClientDisconnected(); // <-- Add this line to notify disconnection
            lock (connectedClients)
            {
                connectedClients.Remove(client);
            }
            client.Close();
        }
    }

    /// <summary>
    /// Broadcasts data to all connected TCP clients.
    /// </summary>
    public void BroadCastData(string data)
    {
        if (!isRunning || tcpListener == null)
        {
            Debug.LogWarning("[ServerManager] Cannot broadcast, server not running.");
            return;
        }

        // Add newline so clients using ReadLine() get the message immediately.
        if (!data.EndsWith("\n"))
            data += "\n";

        byte[] bytes = Encoding.UTF8.GetBytes(data);
        lock (connectedClients)
        {
            foreach (var client in connectedClients)
            {
                if (client.Connected)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(bytes, 0, bytes.Length);
                        try { stream.Flush(); } catch { } // Flush is harmless; some Streams noop.
                        Debug.Log($"[ServerManager] Broadcast wrote {bytes.Length} bytes to {client.Client.RemoteEndPoint}");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[ServerManager] Broadcast to {client.Client.RemoteEndPoint} failed: {ex.Message}");
                    }
                }
            }
        }
        Debug.Log($"[ServerManager] Broadcasted: \"{data.TrimEnd()}\" to all clients.");
    }

    /// <summary>
    /// Sends data to all currently connected TCP clients.
    /// </summary>
    /// <param name="data">The string data to send.</param>
    public void SendDataToClients(string data)
    {
        if (!isRunning || tcpListener == null)
        {
            Debug.LogWarning("[ServerManager] Cannot send data, server not running.");
            return;
        }

        // Add newline so clients using ReadLine() get the message immediately.
        if (!data.EndsWith("\n"))
            data += "\n";

        byte[] bytes = Encoding.UTF8.GetBytes(data);
        lock (connectedClients)
        {
            foreach (var client in connectedClients)
            {
                if (client.Connected)
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(bytes, 0, bytes.Length);
                        try { stream.Flush(); } catch { }
                        Debug.Log($"[ServerManager] Sent {bytes.Length} bytes to {client.Client.RemoteEndPoint}");
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[ServerManager] Send to {client.Client.RemoteEndPoint} failed: {ex.Message}");
                    }
                }
            }
        }
        Debug.Log($"[ServerManager] Sent: \"{data.TrimEnd()}\" to all existing clients.");
    }

    /// <summary>
    /// Convenience method for the inspector editor to call.
    /// </summary>
    public void BroadcastInspectorMessage()
    {
        BroadCastData(inspectorBroadcastMessage);
    }

    /// <summary>
    /// Gets the first local IPv4 address found on the machine.
    /// </summary>
    private string GetLocalIPv4()
    {
        string localIP = "Not found";
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ServerManager] Failed to get local IPv4 address: {ex.Message}");
        }
        return localIP;
    }
}