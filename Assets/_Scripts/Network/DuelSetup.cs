using UnityEngine;
using Mirror; // NEW BASE

public class DuelSetup : NetworkBehaviour // Inherits NetworkBehaviour
{
    // Mirror syntax for synced variables (Server sets, Clients read)
    [SyncVar]
    public bool isReady = false;

    // Called on the Client when the network object is created and ready
    public override void OnStartClient()
    {
        // Check if this instance belongs to the current user
        if (isLocalPlayer)
        {
            // MY BOARD (Big, Left/Center)
            transform.position = new Vector3(-4, 0, 0);
            transform.localScale = Vector3.one;
            gameObject.name = "My_Player";
        }
        else
        {
            // ENEMY BOARD (Mini-View, Right/Side)
            transform.position = new Vector3(12, 5, 0);
            transform.localScale = new Vector3(0.6f, 0.6f, 1f);
            gameObject.name = "Enemy_Player";
        }

        // Note: CheckGameState will be called by the DuelGameManager after all players spawn.
    }

    // --- READY LOGIC ---

    // Public method called by the UI button (will only execute CmdSignalReady if local)
    public void SetReady()
    {
        if (isLocalPlayer) CmdSignalReady();
    }

    // Command sent from Client to Server to update the ready state
    [Command]
    void CmdSignalReady()
    {
        // Server receives the command and updates the Synced Variable
        isReady = true;

        // Tell the server-side manager to check if the game can start
        if (DuelGameManager.instance != null)
            DuelGameManager.instance.CheckGameState();
    }
}