using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingDetailUI : MonoBehaviour
{
    public static BuildingDetailUI Instance;

    [Header("UI References")]
    public Image previewImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;

    public TextMeshProUGUI buildTimeText;

    public Button buildButton;
    public Button cancelButton;

    private BuildingData currentData;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false); // 기본은 안 보이게
    }

    public void Show(BuildingData data)
    {
        currentData = data;
        gameObject.SetActive(true);

        var levelInfo = data.levels[0];

        woodText.text = $"x {levelInfo.requiredWood}";
        stoneText.text = $"x {levelInfo.requiredStone}";
        buildTimeText.text = $"건설 시간: {levelInfo.buildTime}초";

        bool canBuild = HasRequiredResources(data);

        buildButton.interactable = canBuild;

        if (!canBuild)
        {
            buildTimeText.text = "❌ 재료 부족";
        }

        buildButton.onClick.RemoveAllListeners();
        buildButton.onClick.AddListener(() =>
        {
            if (!HasRequiredResources(data)) return;

            // 재료 소비
            SpendMaterials(levelInfo);

            BuildManager.Instance.StartPlacing(data);
            gameObject.SetActive(false);
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private bool HasRequiredResources(BuildingData data)
    {
        var levelInfo = data.levels[0];

        int woodCount = UIInventory.Instance.GetItemCount(levelInfo.woodItem);
        int stoneCount = UIInventory.Instance.GetItemCount(levelInfo.stoneItem);

        return woodCount >= levelInfo.requiredWood &&
               stoneCount >= levelInfo.requiredStone;
    }

    private void SpendMaterials(BuildingData.LevelInfo levelInfo)
    {
        // 나무 제거
        UIInventory.Instance.RemoveItem(levelInfo.woodItem, levelInfo.requiredWood);

        // 돌 제거
        UIInventory.Instance.RemoveItem(levelInfo.stoneItem, levelInfo.requiredStone);
    }
}
