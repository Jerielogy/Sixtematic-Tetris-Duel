using UnityEngine;

public class TetrisBoard : MonoBehaviour
{
    // The Grid Dimensions
    public static int width = 14;
    public static int height = 22;

    // The Data: Stores which blocks are where
    public static Transform[,] grid = new Transform[width, height];

    // Reference for the wall blocks
    public GameObject borderBlockPrefab;

    void Start()
    {
        DrawBorder();
    }

    // Draws the gray U-shape around the board
    void DrawBorder()
    {
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y < height; y++)
            {
                // Draw if Left Wall (-1), Right Wall (10), or Floor (-1)
                if (x == -1 || x == width || y == -1)
                {
                    if (borderBlockPrefab != null)
                        Instantiate(borderBlockPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }
    }

    // Rounds a vector to the nearest whole number (1.0, 2.0)
    public static Vector2 RoundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    // Checks if a position is inside the 10x20 playing area
    public static bool InsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < width && (int)pos.y >= 0);
    }

    // --- ROW CLEARING LOGIC (This fixes your error) ---

    // 1. Delete all blocks in a specific row
    public static void DeleteRow(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    // 2. Move a row down (after the one below was deleted)
    public static void DecreaseRow(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, y] != null)
            {
                // Move logic
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    // 3. Move ALL rows above specific index down
    public static void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < height; ++i)
            DecreaseRow(i);
    }

    // 4. Check if a row is full
    public static bool IsRowFull(int y)
    {
        for (int x = 0; x < width; ++x)
            if (grid[x, y] == null)
                return false;
        return true;
    }

    // 5. The Main Function called by Tetromino.cs
    // Modified DeleteFullRows
    public static void DeleteFullRows()
    {
        int linesClearedInThisFrame = 0; // NEW: Counter

        for (int y = 0; y < height; ++y)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                y--;

                linesClearedInThisFrame++; // NEW: Increment counter
            }
        }

        // NEW: Report to Manager if lines were cleared
        if (linesClearedInThisFrame > 0 && CoreGameManager.instance != null)
        {
            CoreGameManager.instance.RegisterLinesCleared(linesClearedInThisFrame);
        }
    }
}