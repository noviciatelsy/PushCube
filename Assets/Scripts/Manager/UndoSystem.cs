using System.Collections.Generic;
using UnityEngine;

public class UndoSystem : MonoBehaviour
{
    public static UndoSystem Instance;
    Dictionary<MapRoot, int> mapCheckpoints = new Dictionary<MapRoot, int>();

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
            GridManager.Instance.Register(obj);
            obj.SetGridPos(pos);
        }
    }

    // ---------------- SpawnCommand ----------------
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
                obj.SetActive(false); // 删除新生成箱子时隐藏即可
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
        {
            Debug.Log("currentMap NULL");
            return;
        }

        if (!mapCheckpoints.ContainsKey(currentMap))
        {
            Debug.Log("map checkpoint not found");
            return;
        }

        FindObjectOfType<MovementSystem>()?.StopAllMovement();
        int target = mapCheckpoints[currentMap];

        while (history.Count > target)
            Undo();


    }

    //void UndoToCheckpoint()
    //{
    //    if (checkpoints.Count == 0) return;
    //    int target = checkpoints.Peek();
    //    while (history.Count > target)
    //        Undo();
    //}
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
        currentAction.commands.Add(new SpawnCommand(obj.gameObject));
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