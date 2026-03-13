using UnityEngine;

public class CameraFollowMap : MonoBehaviour
{
    public Player player;
    public float smoothTime = 0.15f;

    private Vector3 velocity;
    private Vector3 offset;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<Player>();

        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Transform mapRoot = player.currentMap.transform; // 玩家当前地图

        Vector3 targetPos;

        if (mapRoot != null)
        {
            // 玩家在地图内的位置
            Vector3 localPlayerPos = player.transform.position - mapRoot.position;

            targetPos = mapRoot.position  + offset;
        }
        else
        {
            targetPos = player.transform.position + offset;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}