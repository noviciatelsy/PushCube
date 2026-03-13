using UnityEngine;

public class ElectricGround : PressurePlate
{
    protected override void OnPress()
    {
        base.OnPress();

        var cell = GridManager.Instance.GetCell(GridPos);

        if (cell == null)
            return;

        foreach (var obj in cell.objects)
        {
            if (obj is Player)
            {
                Debug.Log("Electric Ground Triggered");

                UndoSystem.Instance.UndoToMapCheckpoint();
                break;
            }
        }
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }
}