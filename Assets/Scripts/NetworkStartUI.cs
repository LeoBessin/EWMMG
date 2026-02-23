using Unity.Netcode;
using UnityEngine;

public class NetworkStartUI : MonoBehaviour
{
    private GUIStyle buttonStyle;
    private GUIStyle titleStyle;

    void Start()
    {
        // Check for command line argument to auto-launch as server
        string[] args = System.Environment.GetCommandLineArgs();
        foreach (string arg in args)
        {
            if (arg.ToLower() == "-launch-as-server")
            {
                Debug.Log("Auto-launching as server from command line argument");
                
                if (NetworkManager.Singleton == null)
                {
                    Debug.LogError("NetworkManager.Singleton is null! Make sure NetworkManager is in the scene.");
                    return;
                }

                var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
                if (transport != null)
                {
                    // Force server to bind to all interfaces (0.0.0.0)
                    transport.SetConnectionData("0.0.0.0", 7777, "0.0.0.0");
                    Debug.Log($"NetworkManager found. Starting server on 0.0.0.0:7777 (binding to all interfaces)");
                }
                else
                {
                    Debug.LogError("UnityTransport not found!");
                    return;
                }
                
                bool started = NetworkManager.Singleton.StartServer();
                if (started)
                {
                    Debug.Log("Server started successfully!");
                }
                else
                {
                    Debug.LogError("Failed to start server!");
                }
                break;
            }
        }
    }

    void OnGUI()
    {
        // Check if NetworkManager exists
        if (NetworkManager.Singleton == null)
        {
            GUILayout.Label("ERROR: NetworkManager not found in scene!");
            return;
        }

        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(Screen.height * 0.035f),
                fontStyle = FontStyle.Bold
            };
            buttonStyle.normal.textColor = Color.white;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.height * 0.05f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = Color.white;
        }

        // Show connection status
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
        {
            float statusW = Screen.width * 0.3f;
            float statusH = Screen.height * 0.15f;
            float x = (Screen.width - statusW) * 0.5f;
            float y = Screen.height * 0.1f;

            GUI.Box(new Rect(x, y, statusW, statusH), "");
            
            GUILayout.BeginArea(new Rect(x + 10, y + 10, statusW - 20, statusH - 20));
            GUILayout.Label("Connection Status", titleStyle);
            GUILayout.Space(10);
            GUILayout.Label($"Mode: {(NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client")}");
            GUILayout.Label($"Connected: {NetworkManager.Singleton.IsConnectedClient}");
            GUILayout.Label($"Players: {NetworkManager.Singleton.ConnectedClients.Count}");
            
            if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                if (NetworkManager.Singleton.LocalClient != null && NetworkManager.Singleton.LocalClient.PlayerObject == null)
                {
                    GUILayout.Space(10);
                    GUILayout.Label("⚠ No player spawned!", new GUIStyle(GUI.skin.label) { normal = new GUIStyleState { textColor = Color.yellow } });
                    GUILayout.Label("Add Player Prefab to NetworkManager");
                }
            }
            GUILayout.EndArea();
            return;
        }

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            float btnW = Screen.width * 0.25f;
            float btnH = Screen.height * 0.08f;
            float gap = btnH * 0.3f;
            float totalH = btnH + gap + btnH + gap + btnH + gap + btnH;
            float x = (Screen.width - btnW) * 0.5f;
            float y = (Screen.height - totalH) * 0.5f;

            GUI.Label(new Rect(x, y, btnW, btnH), "Join Game", titleStyle);
            y += btnH + gap;

            if (GUI.Button(new Rect(x, y, btnW, btnH), "Host", buttonStyle))
            {
                Debug.Log("Starting Host...");
                NetworkManager.Singleton.StartHost();
            }
            y += btnH + gap;

            if (GUI.Button(new Rect(x, y, btnW, btnH), "Client", buttonStyle))
            {
                var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
                if (transport != null)
                {
                    Debug.Log($"Connecting to {transport.ConnectionData.Address}:{transport.ConnectionData.Port}");
                }
                bool started = NetworkManager.Singleton.StartClient();
                Debug.Log($"StartClient returned: {started}");
            }
            y += btnH + gap;

            if (GUI.Button(new Rect(x, y, btnW, btnH), "Server", buttonStyle))
            {
                Debug.Log("Starting Server...");
                NetworkManager.Singleton.StartServer();
            }
        }
    }
}
