using UnityEngine;

public class MergeBox : Box
{
    public int level = 1;
    public MergeBox nextLevelPrefab;

    // 判断是否可以与 other 合成
    public bool CanMerge(MergeBox other)
    {
        if (other == null)
            return false;

        if (other.level != level)
            return false;

        Debug.Log($"[MERGE CHECK] this={name} pos={GridPos} other={other.name} pos={other.GridPos}");

        Vector2Int dir = other.GridPos - this.GridPos;

        Debug.Log($"[MERGE CHECK] dir={dir}");

        Vector2Int targetPos = other.GridPos + dir;

        Debug.Log($"[MERGE CHECK] target={targetPos}");

        bool hasGround = GridManager.Instance.HasGround(targetPos);
        bool blocked = GridManager.Instance.IsBlocked(targetPos);

        Debug.Log($"[MERGE CHECK] ground={hasGround} blocked={blocked}");

        // 如果后面没有格子，也算无法移动（可以合成）
        if (!GridManager.Instance.HasGround(targetPos))
            return true;

        // 如果后面有格子，但被阻挡，则可以合成
        if (GridManager.Instance.IsBlocked(targetPos))
            return true;

        // 后方格子可移动，则不能合成
        return false;
    }

    // 合成方法：不 Destroy 原箱子，只生成新箱子
    public MergeBox MergeWith(MergeBox other)
    {
        Vector2Int pos = other.GridPos;

        var newBox = Instantiate(nextLevelPrefab);
        newBox.transform.position = new Vector3(pos.x, 0, pos.y);
        newBox.ForceRegister();

        return newBox;
    }
}