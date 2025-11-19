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
    public float duration;
    public float delay;
    public Ease easeType = Ease.OutElastic;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
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
