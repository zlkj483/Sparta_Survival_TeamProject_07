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

        // 기본 정보 표시
        previewImage.sprite = data.icon;
        nameText.text = data.buildingName;
        descriptionText.text = data.description;

        // 0레벨 건축 기준
        var levelInfo = data.levels[0];

        woodText.text = $"x {levelInfo.requiredWood}";
        stoneText.text = $"x {levelInfo.requiredStone}";
        buildTimeText.text = $"건설 시간: {levelInfo.buildTime}초";

        // 버튼 리스너 초기화 후 추가
        buildButton.onClick.RemoveAllListeners();
        buildButton.onClick.AddListener(() =>
        {
            BuildManager.Instance.StartPlacing(data);
            gameObject.SetActive(false);
        });

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
