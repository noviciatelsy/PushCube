using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : GridObject
{
    Player player;
    bool collected = false;

    bool ready = false;

    [Header("旋转设置")]
    public Transform rotateTarget;
    private float rotateSpeed = 90f;

    void Start()
    {
        player = FindObjectOfType<Player>();
        ready = true;
    }

    void Update()
    {
        // 持续旋转
        if (rotateTarget != null)
        {
            Vector3 rot = rotateTarget.eulerAngles;

            rot.y += rotateSpeed * Time.deltaTime;

            rotateTarget.eulerAngles = rot;
        }

        if (!ready) return;
        if (collected) return;
        if (player == null) return;

        if (player.GridPos == GridPos)
        {
            Collect();
        }
    }

    void Collect()
    {
        Debug.Log("oncollect");
        collected = true;

        // 触发 UI 收集
        CollectControl.Instance.Collect();
        SoundManager.Instance.PlaySFX("Pickup");
        // 从 Grid 注销
        GridManager.Instance.Unregister(this);

        // 删除物体
        Destroy(gameObject);
    }

    public override bool IsBlocking()
    {
        return false;
    }
}
