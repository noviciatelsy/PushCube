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
        // µÈ´ý GridManager ³õÊ¼»¯
        while (GridManager.Instance == null)
            yield return null;

        RegisterToGrid();
    }

    void OnDisable()
    {
        if (GridManager.Instance != null)
            GridManager.Instance.Unregister(this);
    }

    void RegisterToGrid()
    {
        Vector3 pos = transform.localPosition;

        Vector2Int localGrid = new Vector2Int(
            Mathf.RoundToInt(pos.x),
            Mathf.RoundToInt(pos.z)
        );

        MapRoot map = GetComponentInParent<MapRoot>();

        if (map != null)
        {
            GridPos = localGrid + map.mapOffset;
            Debug.Log("?");
        }
        else
            GridPos = localGrid;

        GridManager.Instance.Register(this);
    }

    public virtual bool IsBlocking()
    {
        return false;
    }

    public void SetGridPos(Vector2Int pos)
    {
        GridManager.Instance.Unregister(this);

        GridPos = pos;

        transform.position = new Vector3(pos.x, 0, pos.y);

        GridManager.Instance.Register(this);
    }
}