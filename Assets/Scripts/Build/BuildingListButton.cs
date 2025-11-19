using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingListButton : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;

    private BuildingData data;

    public void Setup(BuildingData buildingData)
    {
        data = buildingData;

        iconImage.sprite = data.icon;
        nameText.text = data.buildingName;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OpenDetail);
    }

    private void OpenDetail()
    {
        BuildingDetailUI.Instance.Show(data);
    }
}
