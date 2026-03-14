using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
        if (obj is Ground g)
        {
            var cell = GetOrCreateCell(obj.GridPos);
            cell.ground = g;
        }
        else if (obj is Box box)
        {
            // 注册 box 所有占据格子
            foreach (var pos in box.GetOccupiedCells())
            {
                var cell = GetOrCreateCell(pos);
                if (!cell.objects.Contains(box))
                    cell.objects.Add(box);
            }
        }
        else
        {
            var cell = GetOrCreateCell(obj.GridPos);
            if (!cell.objects.Contains(obj))
                cell.objects.Add(obj);
        }
    }

    // 注销
    public void Unregister(GridObject obj)
    {
        if (obj is Ground)
        {
            var cell = GetCell(obj.GridPos);
            if (cell != null && cell.ground == obj)
                cell.ground = null;

            if (cell != null && cell.ground == null && cell.objects.Count == 0)
                grid.Remove(obj.GridPos);
        }
        else if (obj is Box box)
        {
            // 移除 box 占据的所有格子
            foreach (var pos in box.GetOccupiedCells())
            {
                var cell = GetCell(pos);

                if (cell == null) continue;

                cell.objects.Remove(box);

                if (cell.ground == null && cell.objects.Count == 0)
                    grid.Remove(pos);
            }
        }
        else
        {
            var cell = GetCell(obj.GridPos);
            if (cell != null)
                cell.objects.Remove(obj);
        }
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
        if (cell == null) return true;

        foreach (var obj in cell.objects)
        {
            if (obj.IsBlocking())
            {
                if (obj is Box box)
                {
                    // 检查 box 占据格子
                    if (box.GetOccupiedCells().Contains(pos))
                        return true;
                }
                else
                {
                    return true;
                }
            }
        }

        return false;
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

    public void MoveObject(Box box, Vector2Int target)
    {
        // 移除旧格子
        foreach (var cellPos in box.GetOccupiedCells())
        {
            var cell = GetCell(cellPos);
            if (cell != null)
                cell.objects.Remove(box);
        }

        // 更新 GridPos
        box.GridPos = target;

        // 注册新格子
        foreach (var cellPos in box.GetOccupiedCells())
        {
            var cell = GetOrCreateCell(cellPos);  // 使用 GetOrCreateCell 避免 null
            if (!cell.objects.Contains(box))
                cell.objects.Add(box);
        }
    }

    // 返回当前场景中所有 Box（StickyBox/普通Box）
    public List<Box> GetAllBoxes()
    {
        var result = new List<Box>();

        foreach (var kvp in grid)
        {
            var cell = kvp.Value;
            foreach (var obj in cell.objects)
            {
                if (obj is Box box && !result.Contains(box))
                {
                    result.Add(box);
                }
            }
        }

        return result;
    }

    // 返回目标格子上的 Box（支持多格子 StickyBox）
    public Box GetBoxAt(Vector2Int pos)
    {
        var cell = GetCell(pos);
        if (cell == null) return null;

        foreach (var obj in cell.objects)
        {
            if (obj is Box box)
                return box;
        }

        return null;
    }
}