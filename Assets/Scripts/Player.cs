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
        currentMap = GetComponentInParent<MapRoot>();
    }
}
