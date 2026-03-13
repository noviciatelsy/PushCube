using System.Collections.Generic;
using UnityEngine;

public class Door : GridObject
{
    [Header("绑定压力板")]
    public List<PressurePlate> linkedPlates;

    [Header("门的可视化物体")]
    public Transform cubes;

    [Header("门高度偏移")]
    public Vector3 closedPos = new Vector3(0, -2f, 0);
    public Vector3 openPos = Vector3.zero;

    [Header("动画时间")]
    public float moveTime = 0.1f;

    public bool isOpen = false;
    private Vector3 targetPos;
    private Vector3 startPos;
    private float animProgress = 1f;

    protected override void Awake()
    {
        base.Awake();
        if (cubes != null)
        {
            cubes.localPosition = closedPos;
            startPos = closedPos;
            targetPos = closedPos;
        }

        ForceRegister();
    }

    void Update()
    {
        // ----------------------------
        // 原本绑定板子的检测逻辑保留
        // ----------------------------
        bool allPressed = true;
        if (linkedPlates != null && linkedPlates.Count > 0)
        {
            foreach (var plate in linkedPlates)
            {
                if (plate == null || !plate.pressed)
                {
                    allPressed = false;
                    break;
                }
            }

            if (allPressed != isOpen)
            {
                isOpen = allPressed;
                startPos = cubes.localPosition;
                targetPos = isOpen ? openPos : closedPos;
                animProgress = 0f;
            }
        }

        // ----------------------------
        // 动画插值
        // ----------------------------
        if (cubes != null && animProgress < 1f)
        {
            animProgress += Time.deltaTime / moveTime;
            if (animProgress > 1f) animProgress = 1f;
            cubes.localPosition = Vector3.Lerp(startPos, targetPos, animProgress);
        }
    }

    public override bool IsBlocking()
    {
        return !isOpen;
    }

    // ----------------------------
    // ElectricGroundController 控制接口
    // ----------------------------
    public void SetOpenFromController(bool open)
    {
        if (isOpen == open) return;

        isOpen = open;
        if (cubes != null)
        {
            startPos = cubes.localPosition;
            targetPos = isOpen ? openPos : closedPos;
            animProgress = 0f;
        }
    }
}