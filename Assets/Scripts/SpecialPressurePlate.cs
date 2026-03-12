using UnityEngine;

public class SpecialPressurePlate : PressurePlate
{
    [Header("触发条件")]
    [Tooltip("只触发指定类型")]
    [SerializeField] private bool requireMergeBox = true;

    [Tooltip("MergeBox 触发的等级，<=0 表示不限制等级")]
    [SerializeField] private int requiredLevel = 3;

    public override bool IsValidTrigger(GridObject obj)
    {
        if (obj == null) return false;

        // 类型判断
        if (requireMergeBox)
        {
            if (obj is MergeBox mb)
            {
                // 等级判断
                if (requiredLevel <= 0 || mb.level == requiredLevel)
                {
                    Debug.Log("yesyes");
                    return true;
                }
            }
            return false;
        }
        return true;
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

    protected override void OnPress()
    {
        Debug.Log($"Special Plate Triggered by {requiredLevel}-level MergeBox!");
    }
}