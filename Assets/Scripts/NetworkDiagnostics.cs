using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkDiagnostics : MonoBehaviour
{
    void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("[Diagnostics] NetworkManager.Singleton is NULL!");
            return;
        }

        // Subscribe to connection events
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport == null)
        {
            Debug.LogError("[Diagnostics] UnityTransport component not found!");
            return;
        }

        Debug.Log($"[Diagnostics] Transport Address: {transport.ConnectionData.Address}");
        Debug.Log($"[Diagnostics] Transport Port: {transport.ConnectionData.Port}");
        Debug.Log($"[Diagnostics] Server Listen Address: {transport.ConnectionData.ServerListenAddress}");
    }

    void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }
    }

    void OnServerStarted()
    {
        Debug.Log("[Diagnostics] ✓ Server started successfully!");
    }

    void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[Diagnostics] ✓ Client connected! ClientId: {clientId}");
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log($"[Diagnostics] Total connected clients: {NetworkManager.Singleton.ConnectedClients.Count}");
        }
    }

    void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[Diagnostics] ✗ Client disconnected! ClientId: {clientId}");
    }

    void Update()
    {
        if (NetworkManager.Singleton == null) return;

        if (Time.frameCount % 300 == 0) // Log every 5 seconds at 60fps
        {
            Debug.Log($"[Diagnostics] IsServer: {NetworkManager.Singleton.IsServer}, " +
                     $"IsClient: {NetworkManager.Singleton.IsClient}, " +
                     $"IsHost: {NetworkManager.Singleton.IsHost}, " +
                     $"IsConnectedClient: {NetworkManager.Singleton.IsConnectedClient}, " +
                     $"ConnectedClients: {NetworkManager.Singleton.ConnectedClients.Count}");
        }
    }
}
