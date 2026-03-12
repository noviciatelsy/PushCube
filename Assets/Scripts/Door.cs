using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 感应门，可实时感应压力板，并带 cube 动画
public class Door : GridObject
{
    [Header("绑定压力板")]
    public List<PressurePlate> linkedPlates;

    [Header("动画 Cube")]
    public Transform cube;

    [Header("动画参数")]
    public float animDuration = 0.1f;
    public Vector3 closedPos = new Vector3(0, -2, 0);
    public Vector3 openPos = new Vector3(0, 0, 0);

    private bool isOpen = false;
    private Coroutine animCoroutine;

    protected override void Awake()
    {
        base.Awake();

        if (cube != null)
            cube.localPosition = closedPos; // 初始关闭
    }

    void Update()
    {
        UpdateDoorState();
    }

    void UpdateDoorState()
    {
        bool allPressed = true;
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
            // 门状态变化
            isOpen = allPressed;
            AnimateCube();
        }
    }

    void AnimateCube()
    {
        if (cube == null) return;

        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        Vector3 start = cube.localPosition;
        Vector3 end = isOpen ? openPos : closedPos;

        animCoroutine = StartCoroutine(MoveCube(start, end, animDuration));
    }

    IEnumerator MoveCube(Vector3 start, Vector3 end, float duration)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            cube.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }
        cube.localPosition = end;
    }

    public override bool IsBlocking()
    {
        return !isOpen; // 门关闭阻挡，打开可通行
    }
}