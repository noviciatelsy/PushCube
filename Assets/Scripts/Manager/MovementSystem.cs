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

        // 必须有地面
        if (!GridManager.Instance.HasGround(target))
        {
            //Debug.Log("Cant find ground");
            return;
        }

        Box box = GridManager.Instance.GetObject<Box>(target);

        UndoSystem.Instance.BeginAction();

        if (box != null)
        {
            Vector2Int boxTarget = box.GridPos + dir;

            if (!GridManager.Instance.HasGround(boxTarget))
            {
                UndoSystem.Instance.EndAction();
                return;
            }

            Box frontBox = GridManager.Instance.GetObject<Box>(boxTarget);

            // 合成逻辑
            if (box is MergeBox mb1 && frontBox is MergeBox mb2)
            {
                if (mb1.CanMerge(mb2))
                {
                    UndoSystem.Instance.RecordDestroy(mb1);
                    UndoSystem.Instance.RecordDestroy(mb2);

                    MergeBox newBox = mb1.MergeWith(mb2);

                    UndoSystem.Instance.RecordSpawn(newBox);

                    UndoSystem.Instance.RecordMove(player, player.GridPos);

                    GridManager.Instance.MoveObject(player, target);

                    UndoSystem.Instance.EndAction();
                    return;
                }
            }

            if (GridManager.Instance.IsBlocked(boxTarget))
            {
                UndoSystem.Instance.EndAction();
                return;
            }

            UndoSystem.Instance.RecordMove(box, box.GridPos);

            GridManager.Instance.MoveObject(box, boxTarget);
        }

        UndoSystem.Instance.RecordMove(player, player.GridPos);

        GridManager.Instance.MoveObject(player, target);

        player.UpdateCurrentMap();

        UndoSystem.Instance.EndAction();
    }
}