using System.Collections.Generic;
using UnityEngine;

public class UndoSystem : MonoBehaviour
{
    public static UndoSystem Instance;
    Dictionary<MapRoot, int> mapCheckpoints = new Dictionary<MapRoot, int>();
    public bool IsUndoing { get; private set; }

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

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!MovementSystem.Instance.inputEnabled)
            return;

        if (Input.GetKeyDown(KeyCode.Z))
            Undo();

        if (Input.GetKeyDown(KeyCode.R))
            UndoToMapCheckpoint();
    }

    public void SetCheckpoint()
    {
        checkpoints.Push(history.Count);
    }

    public void UndoToMapCheckpoint()
    {
        Debug.Log("UndoToMapCheckpoint called");

        if (currentMap == null)
            return;

        if (!mapCheckpoints.ContainsKey(currentMap))
            return;
        IsUndoing = true;
        int target = mapCheckpoints[currentMap];

        // 停止移动
        FindObjectOfType<MovementSystem>()?.StopAllMovement();

        // =============================
        // 记录当前状态 (用于Z恢复)
        // =============================
        BeginAction();

        var allObjects = FindObjectsOfType<GridObject>();

        Dictionary<GridObject, Vector2Int> beforePos =
            new Dictionary<GridObject, Vector2Int>();

        foreach (var obj in allObjects)
        {
            if (obj == null) continue;
            if (obj is Ground) continue;
            beforePos[obj] = obj.GridPos;
        }

        // =============================
        // 恢复 checkpoint
        // =============================
        while (history.Count > target)
        {
            var action = history.Pop();

            for (int i = action.commands.Count - 1; i >= 0; i--)
                action.commands[i].Undo();
        }

        // =============================
        // 记录恢复点
        // =============================
        foreach (var kv in beforePos)
        {
            if (kv.Key != null)
                RecordMove(kv.Key, kv.Value);
        }
        var controllers = FindObjectsOfType<ElectricGroundController>();
        foreach (var c in controllers)
            c.SyncState();

        EndAction();
        IsUndoing = false;
        // 更新玩家地图
        Player player = FindObjectOfType<Player>();
        if (player != null)
            player.UpdateCurrentMap();

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