using UnityEngine;
using System.Collections.Generic;

public class PrefabRegistry : MonoBehaviour
{
    public static PrefabRegistry Instance;

    public List<GridObject> prefabs;

    Dictionary<GridObjectType, GridObject> dict =
        new Dictionary<GridObjectType, GridObject>();

    void Awake()
    {
        Instance = this;

        foreach (var p in prefabs)
        {
            dict[p.objectType] = p;
        }
    }

    public GridObject GetPrefab(GridObjectType type)
    {
        return dict[type];
    }
}