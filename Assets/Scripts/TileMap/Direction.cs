using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Direction
{
    public static readonly Vector2Int[] VECTORS =
        {
            Vector2Int.up,
            Vector2Int.left,
            Vector2Int.down,
            Vector2Int.right
        };

    public static readonly Dictionary<Vector2Int, int> VECTOR_INDEX_MAP = new()
        {
            { Vector2Int.up, 0 },
            { Vector2Int.left, 1 },
            { Vector2Int.down, 2 },
            { Vector2Int.right, 3 }
        };

    public int Forward;

    public Direction(int direction)
    {
        Forward = direction;
    }

    public void RotateClockWise(int count)
    {
        Forward = ((Forward + count) % 4 + 4) % 4;
    }
}