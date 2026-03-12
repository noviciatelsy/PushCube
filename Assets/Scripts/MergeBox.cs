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

        // 获取移动方向：假设前方是向 other 的方向
        Vector2Int dir = other.GridPos - this.GridPos;
        Vector2Int targetPos = other.GridPos + dir;

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