using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingUI : MonoBehaviour
{
    public static BuildingUI Instance;

    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI needText;

    private Building target;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Open(Building building, BuildingData data, int level)
    {
        target = building;

        panel.SetActive(true);
        titleText.text = data.buildingName;
        levelText.text = $"Level {level}";
        if (level + 1 < data.levels.Length)
        {
            needText.text = $"Next: Wood {data.levels[level + 1].requiredWood} Stone {data.levels[level + 1].requiredStone}";
        }
        else
        {
            needText.text = "Max Level";
        }
    }

    public void OnUpgradeButton()
    {
        target.Upgrade();
        panel.SetActive(false);
    }

    public void OnRemoveButton()
    {
        target.Remove();
        panel.SetActive(false);
    }
}
