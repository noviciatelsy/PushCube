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
        if (obj == null)
            return false;

        if (requireMergeBox)
        {
            if (obj is MergeBox mb)
            {
                if (requiredLevel <= 0 || mb.level == requiredLevel)
                {
                    //Debug.Log("yesyes");
                    return true;
                }
            }

            return false;
        }

        return true;
    }

    protected override void OnPress()
    {
        Debug.Log($"Special Plate Triggered by {requiredLevel}-level MergeBox!");
    }
}