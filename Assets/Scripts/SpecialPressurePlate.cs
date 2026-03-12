using UnityEngine;

public class SpecialPressurePlate : PressurePlate
{
    public System.Type requiredType;

    public override bool IsValidTrigger(GridObject obj)
    {
        if (requiredType == null)
            return false;

        return obj.GetType() == requiredType || obj.GetType().IsSubclassOf(requiredType);
    }

    protected override void OnPress()
    {
        Debug.Log("Special Plate Triggered");
    }
}