using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Title_DG : MonoBehaviour
{
    private RectTransform rt;

    [Header("최종 목표 Y 위치")]
    public float targetY; // 이미지가 멈출 화면상의 Y 위치 (인스펙터에서 조정)

    [Header("애니메이션 설정")]
    public float duration = 1.0f;
    public float delay = 1.5f;
    public Ease easeType = Ease.OutElastic;

    void Awake()
    {
        rt = GetComponent<RectTransform>();

        // 🛑 초기 위치 설정: 화면 아래로 숨깁니다.
        // 앵커가 중앙이면 -Y값, 앵커가 하단이면 -Height만큼 더 내릴 수 있지만,
        // 단순화를 위해 큰 음수 값으로 숨깁니다. (예: -1000f)
        // 현재 X 위치를 유지하고 Y만 변경
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -1000f);
    }

    void Start()
    {
        // 🛑 타이틀 씬이 시작되면 애니메이션 실행
        rt.DOAnchorPosY(targetY, duration)
          .SetDelay(delay)
          .SetEase(easeType)
          .OnComplete(() => {
              Debug.Log("타이틀 로고 애니메이션 완료.");
          });
    }
}
