using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    // Assign your 7 Tetromino prefabs in the Inspector
    public GameObject[] tetrominoPrefabs;

    // UI REFERENCE: Assign a GameObject in the Inspector to act as the preview position
    public Transform previewSpawnPoint;

    // The currently active preview piece in the scene
    private GameObject previewPieceInstance;

    // --- 7-Bag Randomization System ---
    private List<GameObject> bag = new List<GameObject>();
    // The current piece being previewed (the one that will spawn next)
    private GameObject nextPiecePrefab;

    void Start()
    {
        // On start, fill the bag, pick the first two pieces, and spawn the first one.
        GenerateNewBag();
        SpawnNext();
    }

    // Fills the bag with all 7 pieces and shuffles them (Fisher-Yates)
    void GenerateNewBag()
    {
        bag.Clear();
        bag.AddRange(tetrominoPrefabs);

        // Shuffle the list
        for (int i = 0; i < bag.Count; i++)
        {
            GameObject temp = bag[i];
            int randomIndex = Random.Range(i, bag.Count);
            bag[i] = bag[randomIndex];
            bag[randomIndex] = temp;
        }
    }

    // Gets the next piece from the bag. Generates a new bag if empty.
    GameObject GetNextPiecePrefab()
    {
        if (bag.Count == 0)
        {
            GenerateNewBag();
        }

        // Get the first piece from the shuffled bag
        GameObject nextPiece = bag[0];
        bag.RemoveAt(0);

        return nextPiece;
    }

    // The Main function called by Tetromino.cs when a piece locks
    public void SpawnNext()
    {
        GameObject activePiecePrefab;

        if (nextPiecePrefab == null)
        {
            // On the very first run (Start()), set up the queue.
            activePiecePrefab = GetNextPiecePrefab();
            nextPiecePrefab = GetNextPiecePrefab();
        }
        else
        {
            // All subsequent spawns: the piece that was previewed becomes active
            activePiecePrefab = nextPiecePrefab;
            // Load the piece for the *next* preview
            nextPiecePrefab = GetNextPiecePrefab();
        }

        // 1. Spawn the Active Piece at the Spawner's position
        Instantiate(activePiecePrefab, transform.position, Quaternion.identity);

        // 2. Update the Preview
        UpdatePreview();
    }

    void UpdatePreview()
    {
        // Clean up the old preview piece
        if (previewPieceInstance != null)
        {
            Destroy(previewPieceInstance);
        }

        if (previewSpawnPoint == null)
        {
            Debug.LogError("Preview Spawn Point is not assigned in the Inspector! Cannot show next piece.");
            return;
        }

        // Instantiate the new preview piece
        previewPieceInstance = Instantiate(nextPiecePrefab, previewSpawnPoint.position, Quaternion.identity);

        // Disable the movement script on the preview piece to keep it static
        if (previewPieceInstance.GetComponent<Tetromino>() != null)
        {
            previewPieceInstance.GetComponent<Tetromino>().enabled = false;
        }

        // Set the parent and scale for visual clarity
        previewPieceInstance.transform.parent = previewSpawnPoint;
        previewPieceInstance.transform.localScale = Vector3.one * 0.5f; // Scale down for preview
    }
}