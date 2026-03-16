using System;
using UnityEngine;

public class ElectricGround : PressurePlate
{
    private bool lastPlayerOnPlate = false;
    private bool lastPressed = false;

    public event Action<ElectricGround> OnPressedChanged;

    void LateUpdate()
    {
        CheckPlate();

        if (UndoSystem.Instance != null && UndoSystem.Instance.IsUndoing)
            return;

        if (pressed != lastPressed)
        {
            lastPressed = pressed;
            OnPressedChanged?.Invoke(this);
        }

        CheckPlayerUndo();
    }

    void CheckPlayerUndo()
    {
        if (UndoSystem.Instance != null && UndoSystem.Instance.IsUndoing)
            return;

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

        if (playerOnPlate && !lastPlayerOnPlate)
        {
            UndoSystem.Instance.UndoToMapCheckpoint();
        }

        lastPlayerOnPlate = playerOnPlate;
    }

    protected override void OnRelease()
    {
        base.OnRelease();
        lastPlayerOnPlate = false;
    }

    public override bool IsValidTrigger(GridObject obj)
    {
        return obj is Player || obj is Box;
    }
}