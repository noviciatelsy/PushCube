using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    public Player player;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            TryMove(Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            TryMove(Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            TryMove(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            TryMove(Vector2Int.right);
    }

    void TryMove(Vector2Int dir)
    {
        Vector2Int target = player.GridPos + dir;

        // 1️⃣ 检查目标格子是否有地面
        if (!GridManager.Instance.HasGround(target))
            return;

        // 2️⃣ 检查目标格子是否被阻挡（墙或不可推动箱子）
        GridObject targetObj = GridManager.Instance.GetTopObject(target); // 获取格子上最上层物体
        if (targetObj != null && targetObj.IsBlocking())
        {
            // 如果是箱子，尝试推动
            if (targetObj is Box box)
            {
                Vector2Int boxTarget = box.GridPos + dir;

                // 箱子前方必须有地面
                if (!GridManager.Instance.HasGround(boxTarget))
                    return;

                GridObject boxFront = GridManager.Instance.GetTopObject(boxTarget);

                // 箱子前方阻挡物
                if (boxFront != null)
                {
                    // -------- Merge Logic --------
                    if (box is MergeBox mb1 && boxFront is MergeBox mb2 && mb1.CanMerge(mb2))
                    {
                        // 合成前必须箱子无法移动才允许合成
                        if (boxFront.IsBlocking())
                        {
                            UndoSystem.Instance.BeginAction();

                            UndoSystem.Instance.RecordDestroy(mb1, hideOnly: true);
                            UndoSystem.Instance.RecordDestroy(mb2, hideOnly: true);

                            MergeBox newBox = mb1.MergeWith(mb2);
                            UndoSystem.Instance.RecordSpawn(newBox);

                            UndoSystem.Instance.RecordMove(player, player.GridPos);
                            GridManager.Instance.MoveObject(player, target);

                            UndoSystem.Instance.EndAction();
                            return;
                        }
                        else
                        {
                            // 前方空格不能合成
                            return;
                        }
                    }
                    else
                    {
                        // 箱子前方被墙或其他阻挡，无法推动
                        return;
                    }
                }
                else
                {
                    // 推动箱子
                    UndoSystem.Instance.BeginAction();
                    UndoSystem.Instance.RecordMove(box, box.GridPos);
                    GridManager.Instance.MoveObject(box, boxTarget);

                    UndoSystem.Instance.RecordMove(player, player.GridPos);
                    GridManager.Instance.MoveObject(player, target);
                    player.UpdateCurrentMap();
                    UndoSystem.Instance.EndAction();
                    return;
                }
            }
            else
            {
                // 目标格子被墙或其他阻挡物阻挡，玩家不能移动
                return;
            }
        }

        // 3️⃣ 普通移动玩家
        UndoSystem.Instance.BeginAction();
        UndoSystem.Instance.RecordMove(player, player.GridPos);
        GridManager.Instance.MoveObject(player, target);
        player.UpdateCurrentMap();
        UndoSystem.Instance.EndAction();
    }
}