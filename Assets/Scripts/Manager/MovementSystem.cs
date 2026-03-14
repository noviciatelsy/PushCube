using System.Collections.Generic;
using System.Linq;
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

        if (!GridManager.Instance.HasGround(target))
            return;

        Wall wall = GridManager.Instance.GetObject<Wall>(target);
        Door door = GridManager.Instance.GetObject<Door>(target);
        if (wall != null || (door != null && !door.isOpen))
            return;

        Box box = GridManager.Instance.GetBoxAt(target);

        UndoSystem.Instance.BeginAction();

        if (box != null)
        {
            Vector2Int boxFront = box.GridPos + dir;
            Box frontBox = GridManager.Instance.GetBoxAt(boxFront);

            // =========================
            // 1 先判断 Merge
            // =========================
            if (box is MergeBox mb1 && frontBox is MergeBox mb2 && mb1.CanMerge(mb2))
            {
                Debug.Log("[Merge] Boxes merging");

                UndoSystem.Instance.RecordDestroy(mb1, true);
                UndoSystem.Instance.RecordDestroy(mb2, true);

                MergeBox newBox = mb1.MergeWith(mb2);
                UndoSystem.Instance.RecordSpawn(newBox);

                UndoSystem.Instance.RecordMove(player, player.GridPos);
                GridManager.Instance.MoveObject(player, target);

                UndoSystem.Instance.EndAction();
                return;
            }
            else
            {
                if (box is MergeBox && frontBox is MergeBox)
                {
                    Debug.Log("No1");
                }
                else
                {
                    Debug.Log("No2");
                }
            }

            // =========================
            // 2 推动检测
            // =========================

            if (!CanMoveBox(box, dir))
            {
                UndoSystem.Instance.EndAction();
                return;
            }

            //Vector2Int moveDelta = dir;

            //Vector3 boxStart = box.transform.position;
            //Vector3 boxEnd = boxStart + new Vector3(moveDelta.x, 0, moveDelta.y);

            //UndoSystem.Instance.RecordMove(box, box.GridPos);
            //GridManager.Instance.MoveObject(box, box.GridPos + moveDelta);

            //StartCoroutine(AnimateMove(box.transform, boxStart, boxEnd));
            Vector2Int boxTarget = box.GridPos + dir;

            // 如果前方是冰面，箱子滑行
            if (IsIce(boxTarget))
            {
                boxTarget = GetBoxSlideEnd(boxTarget, dir);
            }

            Vector3 boxStart = box.transform.position;
            Vector3 boxEnd = new Vector3(boxTarget.x, 0, boxTarget.y);

            UndoSystem.Instance.RecordMove(box, box.GridPos);
            GridManager.Instance.MoveObject(box, boxTarget);

            StartCoroutine(AnimateMove(box.transform, boxStart, boxEnd));
        }

        // =========================
        // 玩家滑行
        // =========================

        if (IsIce(target))
        {
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

        // =========================
        // 普通移动
        // =========================

        Vector3 start = player.transform.position;
        Vector3 end = start + new Vector3(dir.x, 0, dir.y);

        UndoSystem.Instance.RecordMove(player, player.GridPos);
        GridManager.Instance.MoveObject(player, target);

        StartCoroutine(AnimateMove(player.transform, start, end));

        player.UpdateCurrentMap();
        UndoSystem.Instance.EndAction();
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

    bool CanMoveBox(Box box, Vector2Int dir)
    {
        var frontCells = GetBoxFrontCells(box, dir);

        Debug.Log($"[CanMoveBox] Trying to move {box.name} dir={dir} frontCells={string.Join(",", frontCells.Select(c => $"({c.x},{c.y})"))}");

        foreach (var pos in frontCells)
        {
            bool hasGround = GridManager.Instance.HasGround(pos);
            bool blocked = GridManager.Instance.IsBlocked(pos);

            Debug.Log($"[CanMoveBox] Checking cell {pos}: HasGround={hasGround}, IsBlocked={blocked}");

            // 每个前沿格子必须有地面
            if (!hasGround)
                return false;

            // 每个前沿格子必须不阻挡
            if (blocked)
                return false;
        }

        return true;
    }

    System.Collections.Generic.List<Vector2Int> GetBoxFrontCells(Box box, Vector2Int dir)
    {
        var occupied = box.GetOccupiedCells();
        var result = new System.Collections.Generic.List<Vector2Int>();

        if (dir == Vector2Int.up)
        {
            int maxY = int.MinValue;
            foreach (var c in occupied) if (c.y > maxY) maxY = c.y;
            foreach (var c in occupied)
                if (c.y == maxY)
                    result.Add(c + dir);
        }
        else if (dir == Vector2Int.down)
        {
            int minY = int.MaxValue;
            foreach (var c in occupied) if (c.y < minY) minY = c.y;
            foreach (var c in occupied)
                if (c.y == minY)
                    result.Add(c + dir);
        }
        else if (dir == Vector2Int.right)
        {
            int maxX = int.MinValue;
            foreach (var c in occupied) if (c.x > maxX) maxX = c.x;
            foreach (var c in occupied)
                if (c.x == maxX)
                    result.Add(c + dir);
        }
        else if (dir == Vector2Int.left)
        {
            int minX = int.MaxValue;
            foreach (var c in occupied) if (c.x < minX) minX = c.x;
            foreach (var c in occupied)
                if (c.x == minX)
                    result.Add(c + dir);
        }

        Debug.Log($"[GetBoxFrontCells] {box.name} dir={dir} frontCells={string.Join(",", result.Select(c => $"({c.x},{c.y})"))}");
        return result;
    }
}