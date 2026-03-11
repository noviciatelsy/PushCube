using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoot : MonoBehaviour
{
    public Vector2Int mapOffset;

    void OnEnable()
    {
        RegisterChildren();
    }

    void RegisterChildren()
    {
        GridObject[] objs = GetComponentsInChildren<GridObject>();

        foreach (var obj in objs)
        {
            obj.gameObject.SetActive(false);
            obj.gameObject.SetActive(true);
        }
    }
}
