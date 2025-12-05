using UnityEngine;
using Mirror;

public class NetworkSpawner : NetworkBehaviour // Inherits NetworkBehaviour
{
    public GameObject[] networkedShapes;
    private bool gameStarted = false;

    // We will use the EnableSpawning() method triggered by the DuelManager.

    public void EnableSpawning()
    {
        // Called by DuelGameManager when both players are ready
        if (isLocalPlayer && !gameStarted)
        {
            gameStarted = true;
            SpawnNext();
        }
    }

    public void SpawnNext()
    {
        // Only the local player instance should initiate the spawn command
        if (!isLocalPlayer || !gameStarted) return;

        // 1. Select Random Shape
        int i = Random.Range(0, networkedShapes.Length);

        // 2. Determine Spawn Position relative to the DuelPlayer container
        Vector3 spawnPos = transform.position + new Vector3(7, 20, 0);

        // 3. Instantiate the Networked Prefab
        // Since this is called by a Client-owned object, Mirror will automatically 
        // give Client Authority to the piece, and sync it to everyone.
        Instantiate(networkedShapes[i], spawnPos, Quaternion.identity);
    }
}