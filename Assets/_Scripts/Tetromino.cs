using UnityEngine;


public class Tetromino : MonoBehaviour
{
    public float fallSpeed = 1.0f;
    private float lastFall = 0;
    private int currentRotationState = 0; // 0, 1, 2, 3 (for 0°, 90°, 180°, 270°)
    private string pieceName;

    void Start()
    {
        // Set piece name for Wall Kick lookup (e.g., "I_Piece", "T_Piece")
        pieceName = gameObject.name;

        // 1. Sync Speed with Manager
        if (CoreGameManager.instance != null)
        {
            fallSpeed = CoreGameManager.instance.currentSpeed;
            if (CoreGameManager.instance.isGameOver)
            {
                Destroy(gameObject);
                return;
            }
        }

        // 2. Check for Instant Game Over (Spawn collision)
        if (!IsValidGridPos())
        {
            if (CoreGameManager.instance != null) CoreGameManager.instance.GameOver();
            Destroy(gameObject);
        }
    }

    void Update()
    {
        
        // Check for Game Over OR Paused
        if (CoreGameManager.instance != null)
        {
            if (CoreGameManager.instance.isGameOver) return;
            if (CoreGameManager.instance.isPaused) return; // <--- ADD THIS LINE
        }
        // Stop if Game Over
        if (CoreGameManager.instance != null && CoreGameManager.instance.isGameOver)
            return;

        // --- LEFT ARROW ---
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);
            if (!IsValidGridPos()) transform.position += new Vector3(1, 0, 0);
            else
            {
                UpdateGrid();
                if (CoreGameManager.instance) CoreGameManager.instance.PlayRotate();
            }
        }

        // --- RIGHT ARROW ---
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);
            if (!IsValidGridPos()) transform.position += new Vector3(-1, 0, 0);
            else
            {
                UpdateGrid();
                if (CoreGameManager.instance) CoreGameManager.instance.PlayRotate();
            }
        }

        // --- UP ARROW (ROTATE) ---
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RotatePiece(1); // 1 = Clockwise (calls the new SRS function)
            if (CoreGameManager.instance) CoreGameManager.instance.PlayRotate();
        }

        // --- SPACEBAR (HARD DROP) - THE FIXED VERSION ---
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            // 1. Play Sound
            if (CoreGameManager.instance) CoreGameManager.instance.PlayRotate();

            // 2. Drop instantly
            while (IsValidGridPos())
            {
                transform.position += new Vector3(0, -1, 0);
            }
            // 3. Step back up to valid spot
            transform.position += new Vector3(0, 1, 0);

            // 4. *** THE FIX *** // We tell the board "I am here now" BEFORE we lock and spawn the next piece.
            // This clears the spawn point so the next piece doesn't crash.
            UpdateGrid();

            // 5. Lock and Spawn Next
            LockBlock();
        }

        // --- DOWN ARROW (OR GRAVITY) ---
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - lastFall >= fallSpeed)
        {
            // Sound only on manual press
            if (Input.GetKeyDown(KeyCode.DownArrow) && CoreGameManager.instance)
                CoreGameManager.instance.PlayRotate();

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
        TetrisBoard.DeleteFullRows();
        enabled = false;
        FindObjectOfType<Spawner>().SpawnNext();
    }

    // --- Inside Tetromino.cs, replace the existing IsValidGridPos() ---

    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
        {
            // 1. Get the rounded coordinates for the grid index
            Vector2 v = new Vector2(Mathf.Round(child.position.x), Mathf.Round(child.position.y));

            int x = (int)v.x;
            int y = (int)v.y;

            // =========================================================================
            // CRITICAL SAFETY CHECK: Ensures indices are valid BEFORE accessing the array.
            // This prevents IndexOutOfRangeException if x < 0, x >= 14, y < 0, or y >= 22.
            // =========================================================================
            if (x < 0 || x >= TetrisBoard.width || y < 0 || y >= TetrisBoard.height)
            {
                // This fails the piece if it is outside the array boundaries
                return false;
            }

            // 2. CHECK FOR COLLISION WITH OTHER BLOCKS (Safe to access the array now)
            if (TetrisBoard.grid[x, y] != null && TetrisBoard.grid[x, y].parent != transform)
            {
                return false;
            }

            // 3. CHECK BORDER (Optional check, but redundant if step 1 is implemented correctly)
            // If your InsideBorder function is still used elsewhere, this confirms the border.
            if (!TetrisBoard.InsideBorder(v))
            {
                return false;
            }
        }
        return true;
    }

    void UpdateGrid()
    {
        // Remove old position from memory
        for (int y = 0; y < TetrisBoard.height; ++y)
            for (int x = 0; x < TetrisBoard.width; ++x)
                if (TetrisBoard.grid[x, y] != null)
                    if (TetrisBoard.grid[x, y].parent == transform)
                        TetrisBoard.grid[x, y] = null;

        // Add new position to memory
        foreach (Transform child in transform)
        {
            Vector2 v = TetrisBoard.RoundVec2(child.position);
            TetrisBoard.grid[(int)v.x, (int)v.y] = child;
        }
    }
    void RotatePiece(int direction)
    {
        // 1. Save the current state and position
        int previousRotationState = currentRotationState;
        Quaternion previousRotation = transform.rotation;
        Vector3 previousPosition = transform.position;

        // 2. Calculate the next rotation state
        // (current + direction + 4) % 4 ensures state cycles 0, 1, 2, 3 correctly
        currentRotationState = (currentRotationState + direction + 4) % 4;

        // 3. Attempt the rotation
        transform.Rotate(0, 0, -90 * direction);

        // 4. Perform the Wall Kick Tests

        // Get the index that maps the state transition (e.g., 0->1) to the kick table
        int kickIndex = GetKickIndex(previousRotationState, currentRotationState);

        // Get the correct kick table (I piece uses different rules)
        Vector2Int[,] kickTable = WallKicks.GetKickTable(pieceName);

        bool rotationSucceeded = false;

        // Loop through the 5 kick tests (0 to 4)
        for (int i = 0; i < kickTable.GetLength(1); i++)
        {
            Vector2Int kickOffset = kickTable[kickIndex, i];

            // Apply the kick offset to the piece's position
            Vector3 newPosition = previousPosition + new Vector3(kickOffset.x, kickOffset.y, 0);
            transform.position = newPosition;

            // Check if the kicked position is valid
            if (IsValidGridPos())
            {
                rotationSucceeded = true;
                break; // Valid position found, stop testing!
            }
        }

        // 5. Finalize or Revert
        if (rotationSucceeded)
        {
            UpdateGrid(); // Update static grid memory with the new, valid position
        }
        else
        {
            // If all kick tests fail, revert all changes
            transform.rotation = previousRotation;
            transform.position = previousPosition;
            currentRotationState = previousRotationState; // Revert state
        }
    }

    // Helper to map the state change to the index in the 6-index kick table
    int GetKickIndex(int prev, int next)
    {
        // NOTE: This mapping is based on how the JLSZT_KICKS array is defined in WallKicks.cs

        // 0 -> R (State 0 to 1) OR 3 -> 0
        if ((prev == 0 && next == 1) || (prev == 3 && next == 0)) return 0;

        // R -> 0 (State 1 to 0) OR 0 -> 3
        if ((prev == 1 && next == 0) || (prev == 0 && next == 3)) return 1;

        // R -> L (State 1 to 2) OR 0 -> R 
        if ((prev == 1 && next == 2) || (prev == 0 && next == 1)) return 2;

        // L -> R (State 2 to 1)
        if (prev == 2 && next == 1) return 3;

        // L -> 0 (State 2 to 3) OR 180 degrees
        if (prev == 2 && next == 3) return 4;

        // 0 -> L (State 3 to 2) 
        if (prev == 3 && next == 2) return 5;

        return 0; // Default fallback
    }

}