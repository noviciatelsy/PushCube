using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    Dictionary<Vector2Int, GridCell> grid =
        new Dictionary<Vector2Int, GridCell>();

    void Awake()
    {
        Instance = this;
    }

    GridCell GetOrCreateCell(Vector2Int pos)
    {
        if (!grid.TryGetValue(pos, out var cell))
        {
            cell = new GridCell();
            grid[pos] = cell;
        }

        return cell;
    }

    public GridCell GetCell(Vector2Int pos)
    {
        grid.TryGetValue(pos, out var cell);
        return cell;
    }

    // 注册
    public void Register(GridObject obj)
    {
        var cell = GetOrCreateCell(obj.GridPos);

        if (obj is Ground g)
        {
            cell.ground = g;
        }
        else
        {
            if (!cell.objects.Contains(obj))
                cell.objects.Add(obj);
        }
    }

    // 注销
    public void Unregister(GridObject obj)
    {
        var cell = GetCell(obj.GridPos);

        if (cell == null)
            return;

        if (obj is Ground)
        {
            if (cell.ground == obj)
                cell.ground = null;
        }
        else
        {
            cell.objects.Remove(obj);
        }

        if (cell.ground == null && cell.objects.Count == 0)
            grid.Remove(obj.GridPos);
    }

    // 是否有地面
    public bool HasGround(Vector2Int pos)
    {
        var cell = GetCell(pos);
        //Debug.Log(cell);
        return cell != null && cell.ground != null;
    }

    // 是否阻挡
    public bool IsBlocked(Vector2Int pos)
    {
        var cell = GetCell(pos);

        if (cell == null)
            return false;

        return cell.IsBlocked();
    }

    // 获取对象
    public T GetObject<T>(Vector2Int pos) where T : GridObject
    {
        var cell = GetCell(pos);

        if (cell == null)
            return null;

        return cell.GetObject<T>();
    }

    // 移动物体
    public void MoveObject(GridObject obj, Vector2Int newPos)
    {
        Unregister(obj);

        obj.GridPos = newPos;
        obj.transform.position = new Vector3(newPos.x, 0, newPos.y);

        Register(obj);
    }
}