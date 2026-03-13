using UnityEngine;

public class ElectricGround : PressurePlate
{
    private bool lastPlayerOnPlate = false;

    protected override void OnPress()
    {
        base.OnPress();

        var cell = GridManager.Instance.GetCell(GridPos);
        if (cell == null) return;

        bool playerOnPlate = false;

        foreach (var obj in cell.objects)
        {
            if (obj is Player)
            {
                playerOnPlate = true;
                break;
            }
        }

        // 玩家踩上板子时触发 Undo，无视 pressed 状态
        if (playerOnPlate && !lastPlayerOnPlate)
        {
            Debug.Log("Electric Ground Triggered by Player");
            UndoSystem.Instance.UndoToMapCheckpoint();
        }

        lastPlayerOnPlate = playerOnPlate;
    }

    protected override void OnRelease()
    {
        base.OnRelease();

        // 玩家离开板子时重置标记
        lastPlayerOnPlate = false;
    }

    // 只要玩家或箱子在板子上，板子就被认为是 pressed
    public override bool IsValidTrigger(GridObject obj)
    {
        return obj is Player || obj is Box;
    }

    void LateUpdate()
    {
        // 先更新 pressed 状态（控制门）
        base.CheckPlate();

        // 再检查玩家触发 Undo
        OnPress();
    }
}