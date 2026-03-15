using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GridObject
{
    public MapRoot currentMap;
    Vector2Int lastGridPos;

    protected override void RegisterToGrid()
    {
        base.RegisterToGrid();
        UpdateCurrentMap();
    }

    void LateUpdate()
    {
        // 只有格子发生变化才更新
        if (GridPos != lastGridPos)
        {
            lastGridPos = GridPos;
            UpdateCurrentMap();
        }
    }

    public void UpdateCurrentMap()
    {
        var cell = GridManager.Instance.GetCell(GridPos);

        if (cell == null)
        {
            Debug.Log("11");
            return;
        }

        if (cell.ground == null)
        {
            Debug.Log("22");
            return;
        }

        //Debug.Log(GridPos);
        MapRoot map = cell.ground.GetComponentInParent<MapRoot>();

        if (map != currentMap)
        {
            currentMap = map;

            Debug.Log("Enter Map: " + map.name);

            //UndoSystem.Instance.SetCheckpoint();
            UndoSystem.Instance.SetCheckpoint(map);
        }
    }
}
