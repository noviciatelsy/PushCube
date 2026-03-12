using UnityEngine;
using System.Collections;

public class GridObject : MonoBehaviour
{
    public Vector2Int GridPos;

    void OnEnable()
    {
        StartCoroutine(RegisterWhenReady());
    }

    IEnumerator RegisterWhenReady()
    {
        while (GridManager.Instance == null)
            yield return null;

        RegisterToGrid();
    }

    void OnDisable()
    {
        if (GridManager.Instance != null)
            GridManager.Instance.Unregister(this);
    }

    //protected virtual void RegisterToGrid()
    //{
    //    Vector3 pos = transform.position;

    //    Vector2Int localGrid = new Vector2Int(
    //        Mathf.RoundToInt(pos.x),
    //        Mathf.RoundToInt(pos.z)
    //    );

    //    MapRoot map = GetComponentInParent<MapRoot>();

    //    if (map != null)
    //        GridPos = localGrid + map.mapOffset;
    //    else
    //        GridPos = localGrid;

    //    GridManager.Instance.Register(this);
    //}
    protected virtual void RegisterToGrid()
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
        GridManager.Instance.MoveObject(this, pos);
    }

    public void ForceRegister()
    {
        if (GridManager.Instance == null)
            return;

        RegisterToGrid();
    }
}