using UnityEngine;

public class MovementSystem : MonoBehaviour
{
    public Player player;
    private float moveDuration = 0.1f;

    bool isMoving = false;
    void Update()
    {
        if (isMoving)
            return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            TryMove(Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            TryMove(Vector2Int.down);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            TryMove(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            TryMove(Vector2Int.right);
    }

    // ==============================
    // [ICE NEW] 判断是否冰面
    // ==============================
    bool IsIce(Vector2Int pos)
    {
        var cell = GridManager.Instance.GetCell(pos);
        if (cell == null) return false;
        return cell.ground is IceGround;
    }

    // ==============================
    // [ICE NEW] 玩家滑行
    // ==============================
    Vector2Int GetPlayerSlideEnd(Vector2Int start, Vector2Int dir)
    {
        Vector2Int pos = start;

        while (true)
        {
            Vector2Int next = pos + dir;

            if (!GridManager.Instance.HasGround(next))
                break;

            if (GridManager.Instance.IsBlocked(next))
                break;

            pos = next;

            if (!IsIce(pos))
                break;
        }

        return pos;
    }

    // ==============================
    // [ICE NEW] 箱子滑行
    // ==============================
    Vector2Int GetBoxSlideEnd(Vector2Int start, Vector2Int dir)
    {
        Vector2Int pos = start;

        while (true)
        {
            Vector2Int next = pos + dir;

            if (!GridManager.Instance.HasGround(next))
                break;

            if (GridManager.Instance.IsBlocked(next))
                break;

            pos = next;

            if (!IsIce(pos))
                break;
        }

        return pos;
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

        UndoSystem.Instance.BeginAction();

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

            // ======================================
            // [ICE MODIFY] 冰面箱子滑行
            // ======================================
            if (IsIce(boxTarget))
            {
                //Vector2Int slideEnd = GetBoxSlideEnd(boxTarget, dir);

                //UndoSystem.Instance.RecordMove(box, box.GridPos);
                //GridManager.Instance.MoveObject(box, slideEnd);

                Vector2Int slideEnd = GetBoxSlideEnd(boxTarget, dir);

                Vector3 boxStart = box.transform.position;
                Vector3 boxEnd = new Vector3(slideEnd.x, 0, slideEnd.y);

                UndoSystem.Instance.RecordMove(box, box.GridPos);
                GridManager.Instance.MoveObject(box, slideEnd);

                StartCoroutine(AnimateMove(box.transform, boxStart, boxEnd));
                // 玩家不移动
                UndoSystem.Instance.EndAction();
                return;
            }

            // -------- 推动普通箱子 --------
            if (frontBox == null && !GridManager.Instance.IsBlocked(boxTarget))
            {
                //UndoSystem.Instance.RecordMove(box, box.GridPos);
                //GridManager.Instance.MoveObject(box, boxTarget);
                Vector3 boxStart = box.transform.position;
                Vector3 boxEnd = new Vector3(boxTarget.x, 0, boxTarget.y);

                UndoSystem.Instance.RecordMove(box, box.GridPos);
                GridManager.Instance.MoveObject(box, boxTarget);

                StartCoroutine(AnimateMove(box.transform, boxStart, boxEnd));
            }
            else if (frontBox != null || GridManager.Instance.IsBlocked(boxTarget))
            {
                // 箱子后面被阻挡，无法推动
                UndoSystem.Instance.EndAction();
                return;
            }

            // Electric ground
            
        }

        // ======================================
        // [ICE MODIFY] 玩家滑行
        // ======================================
        if (IsIce(target))
        {
            //Vector2Int slideEnd = GetPlayerSlideEnd(target, dir);

            //UndoSystem.Instance.RecordMove(player, player.GridPos);
            //GridManager.Instance.MoveObject(player, slideEnd);
            Vector2Int slideEnd = GetPlayerSlideEnd(target, dir);

            Vector3 start1 = player.transform.position;
            Vector3 end1 = new Vector3(slideEnd.x, 0, slideEnd.y);

            UndoSystem.Instance.RecordMove(player, player.GridPos);
            GridManager.Instance.MoveObject(player, slideEnd);

            StartCoroutine(AnimateMove(player.transform, start1, end1));

            player.UpdateCurrentMap();
            UndoSystem.Instance.EndAction();
            return;
        }

        // -------- 移动玩家 --------
        //UndoSystem.Instance.RecordMove(player, player.GridPos);
        //GridManager.Instance.MoveObject(player, target);
        Vector3 start = player.transform.position;
        Vector3 end = new Vector3(target.x, 0, target.y);

        UndoSystem.Instance.RecordMove(player, player.GridPos);
        GridManager.Instance.MoveObject(player, target);

        StartCoroutine(AnimateMove(player.transform, start, end));

        player.UpdateCurrentMap();
        UndoSystem.Instance.EndAction();

        //CheckLandingEffect();

    }

    System.Collections.IEnumerator AnimateMove(Transform obj, Vector3 start, Vector3 end)
    {
        isMoving = true;
        float t = 0;

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = t / moveDuration;

            obj.position = Vector3.Lerp(start, end, p);

            yield return null;
        }

        obj.position = end;

        isMoving = false;

        // 动画结束后检测地面效果
        if (obj == player.transform)
        {
            //CheckLandingEffect();
        }
    }

    System.Collections.IEnumerator AnimateObjects(
    Transform playerT, Vector3 playerStart, Vector3 playerEnd,
    Transform boxT = null, Vector3 boxStart = default, Vector3 boxEnd = default)
    {
        isMoving = true;

        float t = 0;

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = t / moveDuration;

            playerT.position = Vector3.Lerp(playerStart, playerEnd, p);

            if (boxT != null)
                boxT.position = Vector3.Lerp(boxStart, boxEnd, p);

            yield return null;
        }

        playerT.position = playerEnd;

        if (boxT != null)
            boxT.position = boxEnd;

        isMoving = false;
    }

    public void StopAllMovement()
    {
        StopAllCoroutines();
        isMoving = false;
    }
}