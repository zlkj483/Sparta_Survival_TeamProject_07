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
        previewObject = Instantiate(prefab);
        previewObject.name = "PreviewObject";

        var building = previewObject.GetComponent<Building>();
        if (building != null) building.enabled = false;

        foreach (var rb in previewObject.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        foreach (var col in previewObject.GetComponentsInChildren<Collider>())
        {
            col.isTrigger = true;

            if (col is MeshCollider mesh)
            {
                mesh.convex = true;
            }
        }

        foreach (var r in previewObject.GetComponentsInChildren<Renderer>())
        {
            r.material = previewGreen;
            Color c = r.material.color;
            c.a = 0.5f;
            r.material.color = c;
        }

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

    // 프리뷰 위치 갱신
    private void UpdatePreviewPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, 200f, LayerMask.GetMask("Ground")))
        {
            Vector3 pos = hit.point;

            Renderer[] renders = previewObject.GetComponentsInChildren<Renderer>();

            if (renders.Length > 0)
            {
                Bounds bounds = renders[0].bounds;
                for (int i = 1; i < renders.Length; i++)
                    bounds.Encapsulate(renders[i].bounds);

                float bottomY = bounds.min.y;
                float currentY = previewObject.transform.position.y;

                float diff = currentY - bottomY;

                pos.y += diff;
            }

            previewObject.transform.position = pos;
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
            previewObject.transform.rotation
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