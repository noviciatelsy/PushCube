using System.Collections.Generic;
using UnityEngine;

public class UndoSystem : MonoBehaviour
{
    public static UndoSystem Instance;

    struct MoveRecord
    {
        public GridObject obj;
        public Vector2Int pos;
    }

    class ActionRecord
    {
        public List<MoveRecord> moves = new List<MoveRecord>();
    }

    Stack<ActionRecord> history = new Stack<ActionRecord>();

    ActionRecord currentAction;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Undo();
        }
    }

    // 开始一次操作
    public void BeginAction()
    {
        currentAction = new ActionRecord();
    }

    // 记录移动
    public void RecordMove(GridObject obj, Vector2Int pos)
    {
        if (currentAction == null) return;

        currentAction.moves.Add(new MoveRecord
        {
            obj = obj,
            pos = pos
        });
    }

    // 提交操作
    public void EndAction()
    {
        if (currentAction != null && currentAction.moves.Count > 0)
        {
            history.Push(currentAction);
        }

        currentAction = null;
    }

    void Undo()
    {
        if (history.Count == 0)
            return;

        ActionRecord action = history.Pop();

        // 倒序恢复
        for (int i = action.moves.Count - 1; i >= 0; i--)
        {
            var move = action.moves[i];

            GridManager.Instance.MoveObject(move.obj, move.pos);
        }
    }
}