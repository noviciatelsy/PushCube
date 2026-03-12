using System.Collections.Generic;
using UnityEngine;

public class UndoSystem : MonoBehaviour
{
    public static UndoSystem Instance;

    interface IUndoCommand
    {
        void Undo();
    }

    class MoveCommand : IUndoCommand
    {
        GridObject obj;
        Vector2Int pos;

        public MoveCommand(GridObject o, Vector2Int p)
        {
            obj = o;
            pos = p;
        }

        public void Undo()
        {
            if (obj != null)
                GridManager.Instance.MoveObject(obj, pos);
        }
    }

    class DestroyCommand : IUndoCommand
    {
        GameObject prefab;
        Vector2Int pos;

        public DestroyCommand(GameObject p, Vector2Int position)
        {
            prefab = p;
            pos = position;
        }

        public void Undo()
        {
            GameObject obj = Instantiate(prefab);
            obj.transform.position = new Vector3(pos.x, 0, pos.y);
        }
    }

    class SpawnCommand : IUndoCommand
    {
        GameObject obj;

        public SpawnCommand(GameObject o)
        {
            obj = o;
        }

        public void Undo()
        {
            if (obj != null)
                Destroy(obj);
        }
    }

    class ActionRecord
    {
        public List<IUndoCommand> commands = new List<IUndoCommand>();
    }

    Stack<ActionRecord> history = new Stack<ActionRecord>();

    ActionRecord currentAction;

    // ---------- Checkpoint ----------
    Stack<int> checkpoints = new Stack<int>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            Undo();

        if (Input.GetKeyDown(KeyCode.R))
            UndoToCheckpoint();
    }

    public void SetCheckpoint()
    {
        checkpoints.Push(history.Count);
    }

    void UndoToCheckpoint()
    {
        if (checkpoints.Count == 0)
            return;

        int target = checkpoints.Peek();

        while (history.Count > target)
        {
            Undo();
        }
    }

    // ---------- Action ----------

    public void BeginAction()
    {
        currentAction = new ActionRecord();
    }

    public void RecordMove(GridObject obj, Vector2Int pos)
    {
        if (currentAction == null) return;

        currentAction.commands.Add(new MoveCommand(obj, pos));
    }

    public void RecordDestroy(GridObject obj)
    {
        if (currentAction == null) return;

        currentAction.commands.Add(
            new DestroyCommand(obj.gameObject, obj.GridPos)
        );
    }

    public void RecordSpawn(GridObject obj)
    {
        if (currentAction == null) return;

        currentAction.commands.Add(
            new SpawnCommand(obj.gameObject)
        );
    }

    public void EndAction()
    {
        if (currentAction != null && currentAction.commands.Count > 0)
            history.Push(currentAction);

        currentAction = null;
    }

    void Undo()
    {
        if (history.Count == 0)
            return;

        var action = history.Pop();

        for (int i = action.commands.Count - 1; i >= 0; i--)
        {
            action.commands[i].Undo();
        }

        //¹Ø¼ü£ºÎ¬»¤ checkpoint Õ»
        while (checkpoints.Count > 0 && history.Count < checkpoints.Peek())
        {
            checkpoints.Pop();
        }
    }
}