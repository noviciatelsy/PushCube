using System.Collections;
using System.Collections.Generic;
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

        Box box = GridManager.Instance.GetBox(target);

        if (box != null)
        {
            Vector2Int boxTarget = box.GridPos + dir;

            if (GridManager.Instance.IsBlocked(boxTarget))
                return;

            UndoSystem.Instance.RecordMove(box, box.GridPos);

            GridManager.Instance.MoveObject(box, boxTarget);
        }
        else if (GridManager.Instance.IsBlocked(target))
        {
            return;
        }

        UndoSystem.Instance.RecordMove(player, player.GridPos);

        GridManager.Instance.MoveObject(player, target);
    }
}
