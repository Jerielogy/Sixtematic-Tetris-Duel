using UnityEngine;
using Mirror; // NEW BASE

public class DuelGameManager : NetworkBehaviour // Inherits NetworkBehaviour
{
    public static DuelGameManager instance; // Singleton for easy access

    void Awake()
    {
        // Ensures only one referee exists
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Public method called by the DuelSetup script whenever a player's ready state changes
    public void CheckGameState()
    {
        // Only the Server has the authority to check and start the game
        if (!isServer) return;

        // Find all player objects (DuelSetup scripts) in the scene
        var players = FindObjectsOfType<DuelSetup>();

        // Wait until we have 2 players
        if (players.Length < 2) return;

        int readyCount = 0;
        foreach (var p in players)
        {
            // Check the SyncVar value sent from the client
            if (p.isReady) readyCount++;
        }

        // If both players are ready, start the game via ClientRpc
        if (readyCount >= 2)
        {
            RpcStartGame();
        }
    }

    // ClientRpc is run on ALL clients by the Server
    [ClientRpc]
    void RpcStartGame()
    {
        // Logic to hide UI and enable spawners on all machines
        // We assume the Ready UI is hidden/destroyed here.

        // Find ALL spawners and enable block dropping
        NetworkSpawner[] spawners = FindObjectsOfType<NetworkSpawner>();
        foreach (var s in spawners)
        {
            s.EnableSpawning();
        }
        Debug.Log(">>> DUEL STARTED! BLOCKS DROPPING! <<<");
    }

    // NOTE: UI wiring and methods like OnClickReady() must be handled by another script 
    // that calls DuelSetup.SetReady(), as this script only runs on the network.
}