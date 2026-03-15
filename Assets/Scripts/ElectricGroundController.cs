using System.Collections.Generic;
using UnityEngine;

public class ElectricGroundController : MonoBehaviour
{
    public List<ElectricGround> plates;
    public List<Door> linkedDoors;

    public Transform islands;

    public Vector3 closedPos = new Vector3(0, -20f, 0);
    public Vector3 openPos = Vector3.zero;

    public float moveTime = 0.1f;

    Vector3 islandStartPos;
    Vector3 islandTargetPos;
    float islandAnimProgress = 1f;

    bool lastAllPressed = false;

    void Start()
    {
        foreach (var plate in plates)
        {
            if (plate != null)
                plate.OnPressedChanged += OnPlateChanged;
        }

        SyncState();
    }

    void OnDestroy()
    {
        foreach (var plate in plates)
        {
            if (plate != null)
                plate.OnPressedChanged -= OnPlateChanged;
        }
    }

    void OnPlateChanged(ElectricGround plate)
    {
        if (UndoSystem.Instance != null && UndoSystem.Instance.IsUndoing)
            return;

        UpdateState();
    }

    bool CheckAllPressed()
    {
        foreach (var plate in plates)
        {
            if (plate == null || !plate.pressed)
                return false;
        }

        return true;
    }

    void UpdateState()
    {
        bool allPressed = CheckAllPressed();

        if (allPressed == lastAllPressed)
            return;

        lastAllPressed = allPressed;

        foreach (var door in linkedDoors)
        {
            if (door != null)
                door.SetOpenFromController(allPressed);
        }

        if (islands != null)
        {
            islandStartPos = islands.localPosition;
            islandTargetPos = allPressed ? openPos : closedPos;
            islandAnimProgress = 0f;
        }
    }

    void Update()
    {
        if (islands == null) return;

        if (islandAnimProgress < 1f)
        {
            islandAnimProgress += Time.deltaTime / moveTime;

            if (islandAnimProgress > 1f)
                islandAnimProgress = 1f;

            islands.localPosition =
                Vector3.Lerp(islandStartPos, islandTargetPos, islandAnimProgress);
        }
    }


    public void SyncState()
    {
        bool allPressed = CheckAllPressed();

        lastAllPressed = allPressed;

        Vector3 pos = allPressed ? openPos : closedPos;

        if (islands != null)
        {
            islands.localPosition = pos;
            islandStartPos = pos;
            islandTargetPos = pos;
            islandAnimProgress = 1f;
        }

        foreach (var door in linkedDoors)
        {
            if (door != null)
                door.SetOpenFromController(allPressed);
        }
    }
}