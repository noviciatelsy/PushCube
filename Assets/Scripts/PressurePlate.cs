using UnityEngine;

public class PressurePlate : Ground
{
    public bool pressed;

    // 判断对象是否有效触发板子
    public virtual bool IsValidTrigger(GridObject obj)
    {
        return obj != null;
    }

    void LateUpdate()
    {
        CheckPlate();
    }

    public void CheckPlate()
    {
        bool found = false;

        // 检查当前格子
        var cell = GridManager.Instance.GetCell(GridPos);
        if (cell != null)
        {
            foreach (var obj in cell.objects)
            {
                if (IsValidTrigger(obj))
                {
                    found = true;
                    break;
                }

                if (obj is Box box)
                {
                    var occupied = box.GetOccupiedCells();
                    if (occupied.Contains(GridPos) && IsValidTrigger(obj))
                    {
                        found = true;
                        break;
                    }
                }
            }
        }

        // 更新板子状态
        if (found && !pressed)
        {
            pressed = true;
            SoundManager.Instance.PlaySFX("Button");
            OnPress();
        }
        else if (!found && pressed)
        {
            pressed = false;
            OnRelease();
        }
    }

    protected virtual void OnPress()
    {
        //Debug.Log("Pressure Plate Pressed");
    }

    protected virtual void OnRelease()
    {
        Debug.Log("Pressure Plate Released");
    }
}