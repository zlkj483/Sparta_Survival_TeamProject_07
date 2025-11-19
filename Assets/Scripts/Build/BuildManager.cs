using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("Preview Materials")]
    public Material previewGreen;
    public Material previewRed;

    private BuildingData currentData;     // 선택된 건물 데이터
    private GameObject previewObject;     // 미리보기 프리팹
    private bool canBuild = false;        // 설치 가능 여부

    private void Awake()
    {
        Instance = this;
    }

    // 건축 시작 (건물 UI에서 호출)
    public void StartPlacing(BuildingData data)
    {
        if (previewObject != null)
            Destroy(previewObject);

        currentData = data;

        CreatePreview(data.levels[0].prefab);
    }

    // 미리보기 생성
    private void CreatePreview(GameObject prefab)
    {
        previewObject.transform.rotation = Quaternion.Euler(-90, 0, 0);

        previewObject = Instantiate(prefab);
        previewObject.name = "PreviewObject";

        // Building 기능 비활성화 (프리뷰일 때 동작 방지)
        var building = previewObject.GetComponent<Building>();
        if (building != null) building.enabled = false;

        // 모든 Rigidbody 제거 (자식 객체 포함)
        foreach (var rb in previewObject.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        // 모든 Collider를 Trigger로 변경 (자식 포함)
        foreach (var col in previewObject.GetComponentsInChildren<Collider>())
        {
            col.isTrigger = true;

            // MeshCollider라면 Convex 강제
            if (col is MeshCollider mesh)
            {
                mesh.convex = true;
            }
        }

        // 반투명 프리뷰 재질 적용
        foreach (var r in previewObject.GetComponentsInChildren<Renderer>())
        {
            r.material = previewGreen;
            Color c = r.material.color;
            c.a = 0.5f;
            r.material.color = c;
        }

        // 충돌 체크용 스크립트 추가
        previewObject.AddComponent<PreviewCollisionChecker>();
    }


    private void Update()
    {
        if (previewObject == null)
            return;

        UpdatePreviewPosition();
        UpdatePreviewColor();

        if (Input.GetMouseButtonDown(0) && canBuild)
        {
            PlaceBuilding();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    // 미리보기 위치 갱신
    private void UpdatePreviewPosition()
    {
        Transform cam = Camera.main.transform;

        // 플레이어 앞 방향으로 Raycast 쏘기
        if (Physics.Raycast(cam.position, cam.forward, out var hit, 5f))
        {
            // 건축 위치 표시 (거리제한)
            previewObject.transform.position = hit.point;
        }
        else
        {
            // 땅이 없으면 플레이어 앞 5m 지점에 강제 배치 (옵션)
            previewObject.transform.position = cam.position + cam.forward * 5f;
        }
    }

    // 프리뷰 충돌 여부 확인 + 색상 변경
    private void UpdatePreviewColor()
    {
        canBuild = !previewObject.GetComponent<PreviewCollisionChecker>().isColliding;

        Material color = canBuild ? previewGreen : previewRed;

        foreach (var r in previewObject.GetComponentsInChildren<Renderer>())
        {
            Color c = r.material.color;
            r.material = color;
            c.a = 0.5f;
            r.material.color = c;
        }
    }

    // 건물 설치 확정
    private void PlaceBuilding()
    {
        GameObject obj = Instantiate(
            currentData.levels[0].prefab,
            previewObject.transform.position,
            Quaternion.identity
        );

        obj.GetComponent<Building>().Initialize(currentData, 0);

        Destroy(previewObject);
        previewObject = null;
        currentData = null;
    }

    // 배치 취소
    public void Cancel()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
        currentData = null;
    }
}