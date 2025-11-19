using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask groundMask;
    public PlaceableObject buildingPrefab;

    private PlaceableObject previewObject;
    private int currentX, currentY;
    private bool isCurrentValid;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        previewObject = Instantiate(buildingPrefab);
        previewObject.gameObject.name = "PreviewObject";
    }

    private void Update()
    {
        UpdatePreviewPosition();
        HandleRotation();
        HandlePlace();
        HandleDelete();
    }

    private void UpdatePreviewPosition()
    {
        if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000, groundMask))
            return;

        // 플레이어 기준 그리드 좌표 얻기
        GridSystem.Instance.GetXY(hit.point, out int x, out int y);

        if (!GridSystem.Instance.IsValidGridPosition(x, y))
            return;

        currentX = x;
        currentY = y;

        // 플레이어 기준 그리드 -> 월드 좌표
        Vector3 cellCenter = GridSystem.Instance.GetCellCenterWorld(x, y);
        previewObject.transform.position = cellCenter;

        // 같은 칸에 오브젝트 여부 -> 생략 가능 (원한다면 저장 기능 추가)
        isCurrentValid = true;
    }

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
            previewObject.Rotate90();
    }

    private void HandlePlace()
    {
        if (Input.GetMouseButtonDown(0) && isCurrentValid)
        {
            PlaceableObject obj = Instantiate(buildingPrefab);
            obj.transform.position = previewObject.transform.position;
            obj.transform.rotation = previewObject.transform.rotation;
        }
    }

    private void HandleDelete()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // 나중에 "해당 칸의 건물 찾기" 구현 가능
        }
    }
}
