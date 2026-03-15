using UnityEngine;

public class CameraFollowMap : MonoBehaviour
{
    public Player player;
    public float smoothTime = 0.15f;

    private Vector3 velocity;
    private Vector3 offset;
    Camera cam;

    private float borderThreshold = 3f; // 距离边缘触发重置
    private float cameraHalfSize = 5f;  // camera半宽（根据你的正交相机大小）
    public bool enableFollow = true;

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<Player>();

        cam = Camera.main;

        offset = new Vector3(-1.5f,3,-2);
    }

    void LateUpdate()
    {
        if (!enableFollow) return;
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