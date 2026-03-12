using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GridObject
{
    public MapRoot currentMap;

    protected override void RegisterToGrid()
    {
        base.RegisterToGrid();
        UpdateCurrentMap();
    }

    public void UpdateCurrentMap()
    {
        var cell = GridManager.Instance.GetCell(GridPos);

        if (cell == null)
            return;

        if (cell.ground == null)
            return;

        MapRoot map = cell.ground.GetComponentInParent<MapRoot>();

        if (map != currentMap)
        {
            currentMap = map;

            Debug.Log("Enter Map: " + map.name);

            UndoSystem.Instance.SetCheckpoint();
        }
    }
}
