using UnityEngine;

public class NetworkTetrisBoard : MonoBehaviour // Simple MonoBehaviour
{
    // Instance variables, NOT shared. 14 Wide x 22 High.
    public int width = 14;
    public int height = 22;
    public Transform[,] grid;
    public GameObject borderBlockPrefab;

    void Awake()
    {
        // Initialize the unique grid array for THIS instance
        grid = new Transform[width, height];
    }

    void Start()
    {
        // Draw the border immediately
        DrawBorder();
    }

    // --- GRID HELPER FUNCTIONS (Now Instance Methods) ---

    public Vector2 RoundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    public bool InsideBorder(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < width && (int)pos.y >= 0);
    }

    public bool IsRowFull(int y)
    {
        for (int x = 0; x < width; ++x)
            if (grid[x, y] == null) return false;
        return true;
    }

    // --- ROW MANIPULATION ---

    void DeleteRow(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
    }

    void DecreaseRow(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < height; ++i)
        {
            for (int x = 0; x < width; ++x)
            {
                if (grid[x, i] != null)
                {
                    grid[x, i - 1] = grid[x, i];
                    grid[x, i] = null;
                    grid[x, i - 1].position += new Vector3(0, -1, 0);
                }
            }
        }
    }

    public void DeleteFullRows()
    {
        int linesClearedInThisFrame = 0;

        for (int y = 0; y < height; ++y)
        {
            if (IsRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                y--;

                linesClearedInThisFrame++;
            }
        }

        // Score/Attack logic will call a method on PlayerScore.cs here later.
    }

    void DrawBorder()
    {
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y < height; y++)
            {
                if (x == -1 || x == width || y == -1)
                {
                    if (borderBlockPrefab != null)
                    {
                        GameObject wall = Instantiate(borderBlockPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        wall.transform.SetParent(transform, false);
                    }
                }
            }
        }
    }
}