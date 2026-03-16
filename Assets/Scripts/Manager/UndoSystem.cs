using System.Collections.Generic;
using UnityEngine;

public class UndoSystem : MonoBehaviour
{
    public static UndoSystem Instance;
    Dictionary<MapRoot, int> mapCheckpoints = new Dictionary<MapRoot, int>();
    public bool IsUndoing { get; private set; }

    // =============================
    // 长按Z连续撤回
    // =============================
    float undoHoldDelay = 0.3f;      // 长按开始时间
    float undoInterval = 0.15f;       // 连续撤回间隔

    float undoHoldTimer = 0f;
    float undoRepeatTimer = 0f;

    bool undoHolding = false;

    MapRoot currentMap;
    interface IUndoCommand
    {
        void Undo();
    }

    // ---------------- MoveCommand ----------------
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

    // ---------------- DestroyCommand ----------------
    class DestroyCommand : IUndoCommand
    {
        GridObject obj;
        Vector2Int pos;
        bool hideOnly;

        public DestroyCommand(GridObject o, bool hideOnly = false)
        {
            obj = o;
            pos = o.GridPos;
            this.hideOnly = hideOnly;

            GridManager.Instance.Unregister(obj);

            if (!hideOnly)
                GameObject.Destroy(obj.gameObject);
            else
                obj.gameObject.SetActive(false);
        }

        public void Undo()
        {
            if (obj == null) return;

            obj.gameObject.SetActive(true);

            obj.GridPos = pos;
            obj.transform.position = new Vector3(pos.x, 0, pos.y);

            GridManager.Instance.Register(obj);
        }
    }

    // ---------------- SpawnCommand ----------------
    class SpawnCommand : IUndoCommand
    {
        GridObject obj;

        public SpawnCommand(GridObject o)
        {
            obj = o;
        }

        public void Undo()
        {
            if (obj == null) return;

            //关键修复
            GridManager.Instance.Unregister(obj);

            obj.gameObject.SetActive(false);
        }
    }

    // ---------------- ActionRecord ----------------
    class ActionRecord
    {
        public List<IUndoCommand> commands = new List<IUndoCommand>();
    }

    Stack<ActionRecord> history = new Stack<ActionRecord>();
    ActionRecord currentAction;

    Stack<int> checkpoints = new Stack<int>();

    // ---------------- MultiUndoCommand ----------------
    class MultiUndoCommand : IUndoCommand
    {
        List<ActionRecord> undoneActions;

        public MultiUndoCommand(List<ActionRecord> actions)
        {
            undoneActions = actions;
        }

        public void Undo()
        {
            // 重新执行这些Action
            foreach (var action in undoneActions)
            {
                for (int i = 0; i < action.commands.Count; i++)
                {
                    action.commands[i].Undo();
                }
            }
        }
    }
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!MovementSystem.Instance.inputEnabled)
            return;

        HandleUndoInput();

        //if (Input.GetKeyDown(KeyCode.R))
        //    UndoToMapCheckpoint();
    }

    void HandleUndoInput()
    {
        // 松开键
        if (!Input.GetKey(KeyCode.Z))
        {
            undoHolding = false;
            undoHoldTimer = 0f;
            undoRepeatTimer = 0f;
            return;
        }

        // 第一次按下
        if (Input.GetKeyDown(KeyCode.Z))
        {
            undoHolding = true;
            undoHoldTimer = 0f;
            undoRepeatTimer = 0f;

            Undo(); // 立即撤回一次
            return;
        }

        if (!undoHolding)
            return;

        undoHoldTimer += Time.deltaTime;

        // 未到长按时间
        if (undoHoldTimer < undoHoldDelay)
            return;

        undoRepeatTimer += Time.deltaTime;

        if (undoRepeatTimer >= undoInterval)
        {
            undoRepeatTimer = 0f;
            if (history.Count > 0)
                Undo();
        }
    }
    public void SetCheckpoint()
    {
        checkpoints.Push(history.Count);
    }

    public void UndoToMapCheckpoint()
    {
        if (currentMap == null) return;
        if (!mapCheckpoints.ContainsKey(currentMap)) return;

        int target = mapCheckpoints[currentMap];

        if (history.Count <= target) return;

        IsUndoing = true;

        FindObjectOfType<MovementSystem>()?.StopAllMovement();

        List<ActionRecord> undoneActions = new List<ActionRecord>();

        // 连续Undo直到checkpoint
        while (history.Count > target)
        {
            var action = history.Pop();
            undoneActions.Add(action);

            for (int i = action.commands.Count - 1; i >= 0; i--)
                action.commands[i].Undo();
        }

        // 把R操作记录成一个Undo
        BeginAction();
        currentAction.commands.Add(new MultiUndoCommand(undoneActions));
        EndAction();

        var controllers = FindObjectsOfType<ElectricGroundController>();
        foreach (var c in controllers)
            c.SyncState();

        Player player = FindObjectOfType<Player>();
        if (player != null)
            player.UpdateCurrentMap();

        IsUndoing = false;
    }

    public void SetCheckpoint(MapRoot map)
    {
        currentMap = map;

        if (!mapCheckpoints.ContainsKey(map))
        {
            mapCheckpoints[map] = history.Count;
        }
    }

    // ---------------- Action Recording ----------------
    public void BeginAction()
    {
        currentAction = new ActionRecord();
    }

    public void RecordMove(GridObject obj, Vector2Int pos)
    {
        if (currentAction == null) return;
        currentAction.commands.Add(new MoveCommand(obj, pos));
    }

    public void RecordDestroy(GridObject obj, bool hideOnly = false)
    {
        if (currentAction == null) return;
        currentAction.commands.Add(new DestroyCommand(obj, hideOnly));
    }

    public void RecordSpawn(GridObject obj)
    {
        if (currentAction == null) return;

        currentAction.commands.Add(new SpawnCommand(obj));
    }

    public void EndAction()
    {
        if (currentAction != null && currentAction.commands.Count > 0)
            history.Push(currentAction);
        currentAction = null;
    }

    // ---------------- Undo ----------------
    void Undo()
    {
        if (IsUndoing)
            return;

        if (history.Count == 0) return;
        var action = history.Pop();

        for (int i = action.commands.Count - 1; i >= 0; i--)
            action.commands[i].Undo();

        while (checkpoints.Count > 0 && history.Count < checkpoints.Peek())
            checkpoints.Pop();

        Player player = FindObjectOfType<Player>();
        if (player != null)
            player.UpdateCurrentMap();
    }
}