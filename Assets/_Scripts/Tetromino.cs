using UnityEngine;


public class Tetromino : MonoBehaviour
{
    public float fallSpeed = 1.0f;
    private float lastFall = 0;

    void Start()
    {
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
            transform.Rotate(0, 0, -90);
            if (!IsValidGridPos()) transform.Rotate(0, 0, 90);
            else
            {
                UpdateGrid();
                if (CoreGameManager.instance) CoreGameManager.instance.PlayRotate();
            }
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

    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = TetrisBoard.RoundVec2(child.position);

            // Check Borders
            if (!TetrisBoard.InsideBorder(v)) return false;

            // Check other Blocks
            if (TetrisBoard.grid[(int)v.x, (int)v.y] != null && 
                TetrisBoard.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
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
}