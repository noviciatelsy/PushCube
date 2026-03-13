using System.Collections.Generic;
using UnityEngine;

public class StickyBox1 : Box
{
    public Vector2Int size = new Vector2Int(2, 2);

    protected override void RegisterToGrid()
    {
        base.RegisterToGrid();

        Debug.Log($"[StickyBox] Register basePos = {GridPos}");

        foreach (var c in GetOccupiedCells())
        {
            Debug.Log($"[StickyBox] Occupy cell {c}");
        }
    }

    public override List<Vector2Int> GetOccupiedCells()
    {
        List<Vector2Int> cells = new List<Vector2Int>();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                cells.Add(GridPos + new Vector2Int(x, y));
            }
        }

        return cells;
    }
}