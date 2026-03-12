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
        isOpen = false;
        if (cubes != null)
        {
            cubes.localPosition = closedPos;
            targetPos = closedPos;
            startPos = closedPos;
        }

        ForceRegister();
    }

    void Update()
    {
        // 检查压力板状态
        bool allPressed = true;
        if(linkedPlates == null || linkedPlates.Count == 0)
        {
            allPressed = false; // 没有绑定压力板，默认不开门
        }
        foreach (var plate in linkedPlates)
        {
            if (plate == null || !plate.pressed)
            {
                allPressed = false;
                break;
            }
        }

        // 如果开关状态变化，设置动画目标
        if (allPressed != isOpen)
        {
            isOpen = allPressed;
            startPos = cubes.localPosition;
            targetPos = isOpen ? openPos : closedPos;
            animProgress = 0f;
        }

        // 平滑插值移动 cubes
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
}