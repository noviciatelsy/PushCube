using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class GridObject : MonoBehaviour
{
    public Vector2Int GridPos;

    protected virtual void Start()
    {
        Vector3 pos = transform.position;

        GridPos = new Vector2Int(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.z)
        );


        GridManager.Instance.Register(this);
    }

    public virtual bool IsBlocking()
    {
        return false;
    }

    public void SetGridPos(Vector2Int pos)
    {
        GridPos = pos;
        transform.position = new Vector3(pos.x, 0, pos.y);
    }
}
