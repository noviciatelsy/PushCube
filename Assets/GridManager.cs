using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    Dictionary<Vector2Int, List<GridObject>> grid =
        new Dictionary<Vector2Int, List<GridObject>>();

    void Awake()
    {
        Instance = this;
    }

    public void Register(GridObject obj)
    {
        if (!grid.ContainsKey(obj.GridPos))
            grid[obj.GridPos] = new List<GridObject>();

        grid[obj.GridPos].Add(obj);
    }

    public void Unregister(GridObject obj)
    {
        if (!grid.ContainsKey(obj.GridPos))
            return;

        grid[obj.GridPos].Remove(obj);
    }

    public List<GridObject> GetObjects(Vector2Int pos)
    {
        if (!grid.ContainsKey(pos))
            return null;

        return grid[pos];
    }

    public bool IsBlocked(Vector2Int pos)
    {
        var objs = GetObjects(pos);

        if (objs == null)
            return false;

        foreach (var obj in objs)
        {
            if (obj.IsBlocking())
                return true;
        }

        return false;
    }

    public Box GetBox(Vector2Int pos)
    {
        var objs = GetObjects(pos);

        if (objs == null)
            return null;

        foreach (var obj in objs)
        {
            if (obj is Box)
                return obj as Box;
        }

        return null;
    }

    public void MoveObject(GridObject obj, Vector2Int newPos)
    {
        Unregister(obj);

        obj.SetGridPos(newPos);

        Register(obj);
    }
}
