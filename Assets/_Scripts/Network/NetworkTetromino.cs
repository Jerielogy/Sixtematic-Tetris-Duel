using UnityEngine;
using Mirror; // NEW: Using Mirror API

public class NetworkTetromino : NetworkBehaviour // Inherits NetworkBehaviour
{
    // Inherits speed from PlayerScore/GameManager later.
    public float fallSpeed = 1.0f;
    private float lastFall = 0;

    // Reference to the unique board this piece belongs to (Instance)
    private NetworkTetrisBoard myBoard;

    void Start()
    {
        // Find the correct board instance based on proximity (Simple Multi-board fix)
        // We assume the piece spawns near the board it belongs to.
        foreach (var board in FindObjectsOfType<NetworkTetrisBoard>())
        {
            if (Vector3.Distance(transform.position, board.transform.position) < 15f)
            {
                myBoard = board;
                break;
            }
        }

        if (myBoard == null) { Debug.LogError("Board not found! Piece destroyed."); Destroy(gameObject); return; }

        // Initial Game Over Check (using the instance board)
        if (!IsValidGridPos()) { Destroy(gameObject); }
        // ... (Speed initialization from PlayerScore goes here later) ...
    }

    void Update()
    {
        // CORE AUTHORITY CHECK: Only the local player can move this piece
        if (!isLocalPlayer) return;

        // --- INPUT LOGIC ---

        // 1. Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (!IsValidGridPos()) transform.position += new Vector3(1, 0, 0);
            else UpdateGrid();
        }

        // 2. Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            if (!IsValidGridPos()) transform.position += new Vector3(-1, 0, 0);
            else UpdateGrid();
        }

        // 3. Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.Rotate(0, 0, -90);
            if (!IsValidGridPos()) transform.Rotate(0, 0, 90);
            else UpdateGrid();
            // ... (Play Rotate SFX here) ...
        }

        // 4. Hard Drop (Spacebar - if enabled)
        // Note: Logic for Down/Gravity is combined below for efficiency.

        // 5. Normal Fall
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= fallSpeed)
        {
            transform.position += new Vector3(0, -1, 0);

            if (!IsValidGridPos())
            {
                transform.position += new Vector3(0, 1, 0);
                LockBlock();
            }
            else
            {
                UpdateGrid();
            }
            lastFall = Time.time;
        }
    }

    // --- HELPER FUNCTIONS ---

    void LockBlock()
    {
        myBoard.DeleteFullRows();
        enabled = false;

        // Find MY spawner to spawn the next one (Must use a specific NetworkSpawner instance)
        // We will call the Spawner attached to the player's board (the parent object)
        NetworkSpawner mySpawner = GetComponentInParent<NetworkSpawner>();
        if (mySpawner != null)
        {
            mySpawner.SpawnNext();
        }
    }

    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = myBoard.RoundVec2(child.position);

            // Check border and block collision using the instance grid (myBoard.grid)
            if (!myBoard.InsideBorder(v)) return false;

            if (myBoard.grid[(int)v.x, (int)v.y] != null &&
                myBoard.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }

    void UpdateGrid()
    {
        // Clear old positions from the instance grid
        for (int y = 0; y < myBoard.height; ++y)
            for (int x = 0; x < myBoard.width; ++x)
                if (myBoard.grid[x, y] != null && myBoard.grid[x, y].parent == transform)
                    myBoard.grid[x, y] = null;

        // Set new positions on the instance grid
        foreach (Transform child in transform)
        {
            Vector2 v = myBoard.RoundVec2(child.position);
            myBoard.grid[(int)v.x, (int)v.y] = child;
        }
    }
}