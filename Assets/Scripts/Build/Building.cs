using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private BuildingData data;
    private int level;

    public void Initialize(BuildingData d, int startLevel)
    {
        data = d;
        level = startLevel;
        StartCoroutine(BuildProcess());
    }

    IEnumerator BuildProcess()
    {
        float buildTime = data.levels[level].buildTime;
        Debug.Log($"{data.buildingName} Building... {buildTime}s");

        yield return new WaitForSeconds(buildTime);
        Debug.Log($"{data.buildingName} Complete!");
    }

    public void Interact()
    {
        BuildingUI.Instance.Open(this, data, level);
    }

    public void Upgrade()
    {
        if (level + 1 >= data.levels.Length) return;

        level++;
        Debug.Log("Upgrade to Level " + level);

        // 기존 모델 제거
        foreach (Transform t in transform)
        {
            if (t.gameObject.name != "Internal")
                Destroy(t.gameObject);
        }

        // 새 Prefab 적용
        Instantiate(data.levels[level].prefab, transform);
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
}
