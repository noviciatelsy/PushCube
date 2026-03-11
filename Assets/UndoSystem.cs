using System.Collections;
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

    Stack<MoveRecord> history = new Stack<MoveRecord>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
        {
            Undo();
        }
    }

    public void RecordMove(GridObject obj, Vector2Int pos)
    {
        history.Push(new MoveRecord
        {
            obj = obj,
            pos = pos
        });
    }

    void Undo()
    {
        if (history.Count == 0)
            return;

        MoveRecord record = history.Pop();

        GridManager.Instance.MoveObject(record.obj, record.pos);
    }
}
