using UnityEngine;
using System;

public class PressurePlate : Ground
{
    public bool pressed;

    public virtual bool IsValidTrigger(GridObject obj)
    {
        return obj != null;
    }

    void Update()
    {
        CheckPlate();
    }

    void CheckPlate()
    {
        var cell = GridManager.Instance.GetCell(GridPos);

        if (cell == null)
            return;

        bool found = false;

        foreach (var obj in cell.objects)
        {
            if (IsValidTrigger(obj))
            {
                found = true;
                break;
            }
        }

        if (found && !pressed)
        {
            pressed = true;
            OnPress();
        }

        if (!found && pressed)
        {
            pressed = false;
            OnRelease();
        }
    }

    protected virtual void OnPress()
    {
        Debug.Log("Pressure Plate Pressed");
    }

    protected virtual void OnRelease()
    {
        Debug.Log("Pressure Plate Released");
    }
}