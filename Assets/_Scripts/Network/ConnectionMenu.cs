using UnityEngine;
using Mirror; // Essential for Mirror API access
using TMPro; // Needed for the Input Field
using UnityEngine.UI; // Useful for UI elements

public class ConnectionMenu : MonoBehaviour
{
    [Header("Network Prefabs")]
    public GameObject duelManagerPrefab; // Drag DuelManagerPrefab here
    [Header("UI References")]
    public TMP_InputField ipInputField;
    public TextMeshProUGUI statusText;
    public GameObject uiPanel; // Drag the Canvas/Panel here to hide it

    // Mirror uses a static singleton reference for easy access
    private NetworkManager manager;

    void Start()
    {
        // Get the singleton manager instance provided by Mirror
        manager = NetworkManager.singleton;
    }

    public void OnClickHost()
    {
        manager.StartHost();

        // NEW FIX: Spawn the referee (DuelManagerPrefab) here
        if (NetworkServer.active && duelManagerPrefab != null)
        {
            // Spawn the referee, giving the server authority
            NetworkServer.Spawn(Instantiate(duelManagerPrefab));
        }

        manager.onlineScene = "DuelScene"; // Ensure this is set
        if (uiPanel) uiPanel.SetActive(false);
        if (statusText) statusText.text = "<color=green>Host Started! Loading Duel...</color>";
    }

    public void OnClickJoin()
    {
        if (manager == null) return;

        string targetIP = ipInputField.text;

        // 1. Set the Address
        manager.networkAddress = targetIP;

        // 2. Start Client
        manager.StartClient();

        // Hide UI and update status
        if (uiPanel) uiPanel.SetActive(false);
        if (statusText) statusText.text = $"<color=yellow>Attempting to join {targetIP}...</color>";
    }
}