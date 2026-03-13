using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 组合多个电子板控制门，并控制额外Transform上升/下降
/// </summary>
public class ElectricGroundController : MonoBehaviour
{
    [Header("监控的电子板列表")]
    public List<ElectricGround> plates;

    [Header("对应门列表")]
    public List<Door> linkedDoors;

    [Header("额外联动对象（岛屿等）")]
    public Transform islands;

    [Header("岛屿位置偏移")]
    public Vector3 closedPos = new Vector3(0, -20f, 0);
    public Vector3 openPos = Vector3.zero;

    [Header("动画时间")]
    public float moveTime = 0.1f;

    private Vector3 islandStartPos;
    private Vector3 islandTargetPos;
    private float islandAnimProgress = 1f;

    private bool lastAllPressed = false;

    private void Start()
    {
        if (islands != null)
        {
            islands.localPosition = closedPos;
            islandStartPos = closedPos;
            islandTargetPos = closedPos;
        }
    }

    void Update()
    {
        if (plates == null || plates.Count == 0 || linkedDoors == null || linkedDoors.Count == 0)
            return;

        // ----------------------------
        // 检查所有电子板状态
        // ----------------------------
        bool allPressed = true;
        foreach (var plate in plates)
        {
            if (plate == null || !plate.pressed)
            {
                allPressed = false;
                break;
            }
        }

        // ----------------------------
        // 状态变化才触发门和岛屿动画
        // ----------------------------
        if (allPressed != lastAllPressed)
        {
            // 门动画
            foreach (var door in linkedDoors)
            {
                if (door == null) continue;
                door.SetOpenFromController(allPressed);
            }

            // 岛屿动画
            if (islands != null)
            {
                islandStartPos = islands.localPosition;
                islandTargetPos = allPressed ? openPos : closedPos;
                islandAnimProgress = 0f;
            }

            lastAllPressed = allPressed;

            if (allPressed)
                Debug.Log("yes");
        }

        // ----------------------------
        // 平滑插值岛屿动画
        // ----------------------------
        if (islands != null && islandAnimProgress < 1f)
        {
            islandAnimProgress += Time.deltaTime / moveTime;
            if (islandAnimProgress > 1f) islandAnimProgress = 1f;
            islands.localPosition = Vector3.Lerp(islandStartPos, islandTargetPos, islandAnimProgress);
        }
    }
}