using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class ItemSlot : MonoBehaviour,
    IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("데이터")]
    public ItemData item;
    public UIInventory inventory;          // 가능하면 인스펙터에서 안 건드려도 되게 자동 셋업
    public Image icon;
    public TextMeshProUGUI quatityText;
    private Outline outline;

    public int index;
    public bool equipped;
    public int quantity;

    [Header("더블클릭 설정")]
    [SerializeField] private float doubleClickThreshold = 0.25f;
    private float lastClickTime = -1f;

    [Header("드래그 설정")]
    [SerializeField] private Canvas canvas;        // 최상단 Canvas 참조
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 originalPosition;
    private RectTransform parentRect;
    private Vector2 dragOffset;

    #region 초기 셋업 (인스펙터 편의성)

    // 컴포넌트를 처음 붙이거나 Reset 눌렀을 때
    private void Reset()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (icon == null)
            icon = GetComponentInChildren<Image>();

        if (quatityText == null)
            quatityText = GetComponentInChildren<TextMeshProUGUI>();

        if (outline == null)
            outline = GetComponent<Outline>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (inventory == null)
            inventory = GetComponentInParent<UIInventory>();
    }

    // 인스펙터에서 값 바뀔 때 자동 보정
    private void OnValidate()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (outline == null)
            outline = GetComponent<Outline>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        if (inventory == null)
            inventory = GetComponentInParent<UIInventory>();
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        outline = GetComponent<Outline>();

        parentRect = rectTransform.parent as RectTransform;
    }

    private void OnEnable()
    {
        if (outline != null)
            outline.enabled = equipped;
    }

    #endregion

    #region 슬롯 표시/정리

    public void Set()
    {
         icon.sprite = item.icon;

        if (quatityText != null)
            quatityText.text = quantity > 1 ? quantity.ToString() : string.Empty;

        if (outline != null)
            outline.enabled = equipped;

        if (icon != null && item != null)
        {
            icon.gameObject.SetActive(true);
            //icon.sprite = item.itemImage;
        }
    }

    public void Clear()
    {
        item = null;

        if (quatityText != null)
            quatityText.text = string.Empty;

        if (icon != null)
            icon.gameObject.SetActive(false);

        equipped = false;
        if (outline != null)
            outline.enabled = false;
    }

    public void OnClickButton()
    {
        if (inventory != null)
            inventory.SelectItem(index);
    }

    public void DecreaseQuantity(int value)
    {
        quantity -= value;
        if (quantity <= 0)
        {
            Clear();
        }
        else
        {
            if (quatityText != null)
                quatityText.text = quantity.ToString();
        }
    }

    #endregion

    #region 더블클릭 처리

    public void OnPointerClick(PointerEventData eventData)
    {
        if (inventory == null)
            return;

        float now = Time.unscaledTime;

        // 더블클릭
        if (now - lastClickTime <= doubleClickThreshold)
        {
            if (item != null)
                inventory.OnSlotDoubleClick(this);

            lastClickTime = -1f;
            return;
        }

        // 🔹 싱글클릭 → SelectItem
        inventory.SelectItem(index);

        lastClickTime = now;
    }

    #endregion

    #region 드래그/드랍 처리

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null)
            return;

        originalPosition = rectTransform.anchoredPosition;

        if (parentRect == null)
            parentRect = rectTransform.parent as RectTransform;

        // 🔹 드래그 시작 시, 마우스의 로컬 좌표 구하기
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 mouseLocalPos);

        // 🔹 슬롯 위치 - 마우스 위치 = 오프셋 저장
        dragOffset = rectTransform.anchoredPosition - mouseLocalPos;

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0.7f;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null)
            return;

        if (parentRect == null)
            parentRect = rectTransform.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 mouseLocalPos);

        // 🔹 드래그 시작 때 저장해 둔 오프셋을 계속 유지
        rectTransform.anchoredPosition = mouseLocalPos + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null)
        {
            ResetPosition();
            return;
        }

        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        var inv = inventory;
        if (inv == null || inv.inventoryArea == null)
        {
            ResetPosition();
            return;
        }

        bool isInsideInventory =
            RectTransformUtility.RectangleContainsScreenPoint(
                inv.inventoryArea,
                eventData.position,
                eventData.pressEventCamera);

        if (!isInsideInventory)
        {
            // 🔹 인벤토리 밖 → 아이템만 버리기
            inv.OnSlotDropOutside(this);
        }

        // 🔹 안/밖 상관없이 슬롯은 항상 원래 자리로
        ResetPosition();
    }

    private void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }

    #endregion
}
