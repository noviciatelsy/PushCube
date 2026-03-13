using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 组合多个电子板控制门
/// </summary>
public class ElectricGroundController : MonoBehaviour
{
    [Header("监控的电子板列表")]
    public List<ElectricGround> plates;

    [Header("对应门列表")]
    public List<Door> linkedDoors;

    private bool lastAllPressed = false;

    void Update()
    {
        if (plates == null || plates.Count == 0 || linkedDoors == null || linkedDoors.Count == 0)
            return;

        bool allPressed = true;
        foreach (var plate in plates)
        {
            if (plate == null || !plate.pressed)
            {
                allPressed = false;
                break;
            }
        }

        // 只有状态变化才调用 SetOpenFromController，避免干扰普通 Door+PressurePlate
        if (allPressed != lastAllPressed)
        {
            foreach (var door in linkedDoors)
            {
                if (door == null) continue;
                door.SetOpenFromController(allPressed);
            }

            if (allPressed)
                Debug.Log("yes");

            lastAllPressed = allPressed;
        }
    }
}