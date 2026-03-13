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

        // 目标格子必须有地面
        if (!GridManager.Instance.HasGround(target))
        {
            Debug.Log("1");
            return;
        }

        Wall wall = GridManager.Instance.GetObject<Wall>(target);
        Door door = GridManager.Instance.GetObject<Door>(target);
        if (wall != null)
        {
            Debug.Log("2");
            return;
        }
        if (door != null && !door.isOpen)
        {
            Debug.Log("3");
            return;
        }
        Box box = GridManager.Instance.GetObject<Box>(target);

        

        // -------- 玩家前方有箱子 --------
        if (box != null)
        {
            Vector2Int boxTarget = box.GridPos + dir;

            // 箱子前方有地面才能尝试推动
            if (!GridManager.Instance.HasGround(boxTarget))
            {
                UndoSystem.Instance.EndAction();
                return;
            }

            Box frontBox = GridManager.Instance.GetObject<Box>(boxTarget);

            // -------- 合成逻辑 --------
            if (box is MergeBox mb1 && frontBox is MergeBox mb2 && mb1.CanMerge(mb2))
            {
                // 合成前必须箱子无法移动才允许合成
                if (GridManager.Instance.IsBlocked(boxTarget))
                {
                    // 隐藏旧箱子
                    UndoSystem.Instance.RecordDestroy(mb1, hideOnly: true);
                    UndoSystem.Instance.RecordDestroy(mb2, hideOnly: true);

                    // 生成新箱子
                    MergeBox newBox = mb1.MergeWith(mb2);
                    UndoSystem.Instance.RecordSpawn(newBox);

                    // 移动玩家
                    UndoSystem.Instance.RecordMove(player, player.GridPos);
                    GridManager.Instance.MoveObject(player, target);

                    UndoSystem.Instance.EndAction();
                    return;
                }
                else
                {
                    // 后面有空地，不能合成，只能推动
                    frontBox = null;
                }
            }

            // -------- 推动普通箱子 --------
            if (frontBox == null && !GridManager.Instance.IsBlocked(boxTarget))
            {
                UndoSystem.Instance.RecordMove(box, box.GridPos);
                GridManager.Instance.MoveObject(box, boxTarget);
            }
            else if (frontBox != null || GridManager.Instance.IsBlocked(boxTarget))
            {
                // 箱子后面被阻挡，无法推动
                UndoSystem.Instance.EndAction();
                return;
            }

            // Electric ground
            
        }
        else Debug.Log("4");

        // -------- 移动玩家 --------
        UndoSystem.Instance.RecordMove(player, player.GridPos);
        GridManager.Instance.MoveObject(player, target);

        player.UpdateCurrentMap();
        UndoSystem.Instance.EndAction();

        UndoSystem.Instance.BeginAction();
        void CheckLandingEffect()
        {
            var cell = GridManager.Instance.GetCell(player.GridPos);
            if (cell == null) return;

            // 检测是否是电击格
            if (cell.ground is ElectricGround electric)
            {
                electric.OnPlayerStep(player.GridPos);
            }
        }

        void TryMove(Vector2Int dir, MovementSystem movementSystem)
        {
            Vector2Int newPos = player.GridPos + dir;

            if (!GridManager.Instance.HasGround(newPos)) return;
            if (GridManager.Instance.IsBlocked(newPos)) return;

            UndoSystem.Instance.BeginAction();
            UndoSystem.Instance.RecordMove(moveobject, player.GridPos);   // 记录移动前位置

            GridManager.Instance.MoveObject(this, newPos);

            CheckLandingEffect();  // ← 移动后检测特殊格

            UndoSystem.Instance.EndAction();
        }
    }
}