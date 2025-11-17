using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float maxValue;
    public float startValue;
    public float passiveValue;
    public Image uiBar;

    private void Start()
    {
        if (uiBar == null)
        {
            // Condition 오브젝트 밑에 Image 자동 연결
            uiBar = transform.Find("Image")?.GetComponent<Image>();
            if (uiBar == null)
                Debug.LogWarning("uiBar Image를 찾을 수 없습니다!", this);
        }

        curValue = startValue;
    }

    private void Update()
    {
        uiBar.fillAmount = GetPercentage();
    }

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }

    public float GetPercentage()
    {
        return curValue / maxValue;
    }
}