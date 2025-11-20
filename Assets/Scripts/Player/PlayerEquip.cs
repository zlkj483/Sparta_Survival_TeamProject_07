using UnityEngine;

public class PlayerEquip : MonoBehaviour
{
    private GameObject currentEquip; // 현재 장착 중인 장비

    /// <summary>
    /// 아이템 장착
    /// </summary>
    /// <param name="item">장착할 아이템 데이터</param>
    /// <param name="equipPoint">장착 위치 Transform</param>
    public void EquipItem(ItemData item, Transform equipPoint)
    {
        // 기존 장비 제거
        if (currentEquip != null)
        {
            Destroy(currentEquip);
            currentEquip = null;
        }

        // 아이템 타입 확인, Equipable인지 체크
        if (item == null || item.type != ItemType.Equipable || item.equipPrefab == null)
            return;

        // equipPrefab 생성 후 equipPoint에 붙이기
        currentEquip = Instantiate(item.equipPrefab, equipPoint);
        currentEquip.transform.localPosition = Vector3.zero;
        currentEquip.transform.localRotation = Quaternion.identity;

        Debug.Log($"장착 완료: {item.displayName}");
    }

    /// <summary>
    /// 장착 해제
    /// </summary>
    public void Unequip()
    {
        if (currentEquip != null)
        {
            Destroy(currentEquip);
            currentEquip = null;
            Debug.Log("장비 해제됨");
        }
    }
}
