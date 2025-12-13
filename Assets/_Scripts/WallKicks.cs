using UnityEngine;

public static class WallKicks
{
    // The standard set of kick offsets used for J, L, S, T, Z pieces.
    // Index 0: 0 -> R (Rotation State 0 to 1)
    // Index 1: R -> 0 (Rotation State 1 to 0)
    // Index 2: R -> L (Rotation State 1 to 2)
    // Index 3: L -> R (Rotation State 2 to 1)
    // Index 4: L -> 0 (Rotation State 2 to 3)
    // Index 5: 0 -> L (Rotation State 3 to 2)
    // Index 6: 0 -> 2 (Rotation State 3 to 0) <--- NOTE: Correct indexing can be complex.
    // We use a simplified set for common standard rotation tests:

    // Kick tests used for J, L, S, T, Z pieces (non-I)
    public static readonly Vector2Int[,] JLSZT_KICKS = new Vector2Int[,]
    {
        // 0 -> R (State 0 to 1)
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -2), new Vector2Int(-1, -2) }, 
        // R -> 0 (State 1 to 0)
        { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, 2), new Vector2Int(1, 2) },
        // R -> L (State 1 to 2)
        { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2) },
        // L -> R (State 2 to 1)
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        // L -> 0 (State 2 to 3)
        { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2) },
        // 0 -> L (State 3 to 0)
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
    };

    // Kick tests used for the I piece (requires bigger kicks)
    public static readonly Vector2Int[,] I_KICKS = new Vector2Int[,]
    {
        // 0 -> R (State 0 to 1)
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int(1, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2) },
        // R -> 0 (State 1 to 0)
        { new Vector2Int(0, 0), new Vector2Int(2, 0), new Vector2Int(-1, 0), new Vector2Int(2, 1), new Vector2Int(-1, -2) },
        // R -> L (State 1 to 2)
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 2), new Vector2Int(2, -1) },
        // L -> R (State 2 to 1)
        { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(-2, 0), new Vector2Int(1, -2), new Vector2Int(-2, 1) },
        // L -> 0 (State 2 to 3)
        { new Vector2Int(0, 0), new Vector2Int(2, 0), new Vector2Int(-1, 0), new Vector2Int(2, 1), new Vector2Int(-1, -2) },
        // 0 -> L (State 3 to 0)
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int(1, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2) },
    };

    // Helper to select the right kick table
    public static Vector2Int[,] GetKickTable(string pieceName)
    {
        if (pieceName == "I_Piece") // Assuming your I piece GameObject is named "I_Piece"
        {
            return I_KICKS;
        }
        return JLSZT_KICKS;
    }
}