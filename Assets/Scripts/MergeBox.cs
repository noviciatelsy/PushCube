using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeBox : Box
{
    public int level = 1;

    public MergeBox nextLevelPrefab;

    public bool CanMerge(MergeBox other)
    {
        if (other == null)
            return false;

        return other.level == level;
    }

    public MergeBox MergeWith(MergeBox other)
    {
        Vector2Int pos = other.GridPos;

        Destroy(other.gameObject);
        Destroy(gameObject);

        var newBox = Instantiate(nextLevelPrefab);
        newBox.transform.position = new Vector3(pos.x, 0, pos.y);

        return newBox;
    }
}
