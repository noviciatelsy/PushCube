using UnityEngine;

public class ElectricGround : Ground
{
    // 可在Inspector中设置原点，默认(0,0)
    public Vector2Int originPos = Vector2Int.zero;

    // 触发电击效果，由Player调用
    public void OnPlayerStep(GridObject player)
    {
        // 记录Undo：先记录player当前位置，再移动
        UndoSystem.Instance.RecordMove(player, player.GridPos);
        GridManager.Instance.MoveObject(player, originPos);
    }
}