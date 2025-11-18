using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public Material previewGreen;
    public Material previewRed;

    private BuildingData currentData;
    private GameObject previewObject;
    private bool canBuild;

    private void Awake()
    {
        Instance = this;
    }

    public void StartPlacing(BuildingData data)
    {
        currentData = data;
        CreatePreview(data.levels[0].prefab);
    }

    private void CreatePreview(GameObject prefab)
    {
        previewObject = Instantiate(prefab);

        foreach (var r in previewObject.GetComponentsInChildren<Renderer>())
            r.material = previewGreen;   // 기본 초록

        // 건물 기능 꺼놓기
        previewObject.GetComponent<Collider>().enabled = false;
        previewObject.GetComponent<Building>().enabled = false;
    }

    private void Update()
    {
        if (previewObject == null)
            return;

        // 마우스 위치 계속 따라다님
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 100f))
        {
            previewObject.transform.position = hit.point;

            // 충돌 체크
            canBuild = CheckCanBuild();
            SetPreviewColor(canBuild);
        }

        // 배치 확정
        if (Input.GetMouseButtonDown(0) && canBuild)
        {
            PlaceBuilding();
        }

        // 취소
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
    }

    private bool CheckCanBuild()
    {
        Collider[] hits = Physics.OverlapBox(
            previewObject.transform.position,
            previewObject.transform.localScale / 2f,
            previewObject.transform.rotation
        );

        foreach (var col in hits)
        {
            if (!col.CompareTag("Ground"))
                return false;
        }

        return true;
    }

    private void SetPreviewColor(bool isGreen)
    {
        Material mat = isGreen ? previewGreen : previewRed;

        foreach (var r in previewObject.GetComponentsInChildren<Renderer>())
        {
            r.material = mat;
        }
    }
    
    private void PlaceBuilding()
    {
        GameObject obj = Instantiate(currentData.levels[0].prefab,
                                     previewObject.transform.position,
                                     Quaternion.identity);

        obj.GetComponent<Building>().Initialize(currentData, 0);

        Destroy(previewObject);
        previewObject = null;
        currentData = null;
    }

    public void Cancel()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
    }
}
