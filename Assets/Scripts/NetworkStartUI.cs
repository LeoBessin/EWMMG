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
                NetworkManager.Singleton.StartServer();
                break;
            }
        }
    }

    void OnGUI()
    {
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
                NetworkManager.Singleton.StartHost();
            y += btnH + gap;

            if (GUI.Button(new Rect(x, y, btnW, btnH), "Client", buttonStyle))
                NetworkManager.Singleton.StartClient();
            y += btnH + gap;

            if (GUI.Button(new Rect(x, y, btnW, btnH), "Server", buttonStyle))
                NetworkManager.Singleton.StartServer();
        }
    }
}
