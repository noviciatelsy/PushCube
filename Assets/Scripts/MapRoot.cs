using UnityEngine;

public class MapRoot : MonoBehaviour
{
    public Vector2Int mapOffset;

    void Start()
    {
        RegisterAll();
    }

    public void RegisterAll()
    {
        GridObject[] objs = GetComponentsInChildren<GridObject>(true);

        foreach (var obj in objs)
        {
            obj.ForceRegister();
        }
    }
}