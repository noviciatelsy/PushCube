using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GridObjectType
{
    Player,
    Box,
    MergeBox1,
    MergeBox2,
    MergeBox3,
    Wall,
    Ground,
    IceGround,
    ElectricGround,
    Door
}

public class GridObject : MonoBehaviour
{
    public Vector2Int GridPos;
    public GridObjectType objectType;

    protected virtual void Awake()
    {
       
    }

    public virtual List<Vector2Int> GetOccupiedCells()
    {
        return new List<Vector2Int>() { GridPos };
    }

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