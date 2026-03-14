using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TransBox : GridObject
{
    [Header("配对传送箱")]
    public TransBox pair;

    public override bool IsBlocking()
    {
        // 默认像墙一样阻挡
        return true;
    }

    /// <summary>
    /// 尝试传送
    /// </summary>
    /// <param name="enterDir">进入方向</param>
    /// <param name="resultPos">传送后的目标位置</param>
    /// <returns>是否成功传送</returns>
   public bool TryTeleport(bool isPlayer,Vector2Int enterDir, out Vector2Int resultPos)
    {
        resultPos = GridPos;

        if (pair == null)
            return false;

        // 异侧出口
        Vector2Int exitPos = pair.GridPos + enterDir;

        // 出口必须有地面
        if (!GridManager.Instance.HasGround(exitPos))
            return false;

        // 出口格子是否被阻挡
        Box frontBox = GridManager.Instance.GetBoxAt(exitPos);
        if (frontBox != null)
        {
            if(!isPlayer) return false;
            else
            {
                bool canboxmove = MovementSystem.Instance.CanMoveBox(frontBox, enterDir);
                if (canboxmove)
                {
                    Vector2Int boxTarget = frontBox.GridPos + enterDir;

                    // 如果前方是冰面，箱子滑行
                    if (MovementSystem.Instance.IsIce(boxTarget))
                    {
                        boxTarget = MovementSystem.Instance.GetBoxSlideEnd(boxTarget, enterDir);
                    }

                    Vector3 boxStart = frontBox.transform.position;
                    Vector3 boxEnd = new Vector3(boxTarget.x, 0, boxTarget.y);

                    UndoSystem.Instance.RecordMove(frontBox, frontBox.GridPos);
                    GridManager.Instance.MoveObject(frontBox, boxTarget);

                    MovementSystem.Instance.MoveBox(frontBox, boxStart, boxEnd);
                    resultPos = exitPos;
                    return true;
                }
                else return false;
            }
        }

        // 墙和门判断保持原逻辑
        Wall wall = GridManager.Instance.GetObject<Wall>(exitPos);
        Door door = GridManager.Instance.GetObject<Door>(exitPos);
        if (wall != null || (door != null && !door.isOpen))
            return false;

        resultPos = exitPos;
        return true;
    }
}