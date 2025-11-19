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

        Collider col = previewObject.GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        foreach (var r in previewObject.GetComponentsInChildren<Renderer>())
        {
            r.material = previewGreen;
            r.material.SetFloat("_Mode", 3);
            r.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            r.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            r.material.SetInt("_ZWrite", 0);
            r.material.DisableKeyword("_ALPHATEST_ON");
            r.material.EnableKeyword("_ALPHABLEND_ON");
            r.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            r.material.renderQueue = 3000;
            Color c = r.material.color;
            c.a = 0.6f;
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

    // 미리보기 위치 갱신
    private void UpdatePreviewPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit, 200f))
        {
            previewObject.transform.position = hit.point;
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