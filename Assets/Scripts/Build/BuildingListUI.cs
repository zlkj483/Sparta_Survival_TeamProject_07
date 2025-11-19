using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingListUI : MonoBehaviour
{
    public Transform content;
    public GameObject buttonPrefab;
    public BuildingData[] buildingList;

    private void Start()
    {
        foreach (var data in buildingList)
        {
            GameObject btn = Instantiate(buttonPrefab, content);
            btn.GetComponent<BuildingListButton>().Setup(data);
        }
    }
}
