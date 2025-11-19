using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Structure : MonoBehaviour, IGatherable
{
    public StructureData data;
    private float accumulatedDamage = 0;
    private float hp;
    private int dropItem;
    protected Vector3 pos;    // 원래 위치
    protected Quaternion rot; // 원래 회전
    public GameObject prefab;       // 재생성할 프리팹

    private void Start()
    {
        hp = data.maxHp;
        dropItem = data.dropCount;
    }
    //맞은위치 , 맞은법선 , 데미지량
    public void Gather(Vector3 hitPoint, Vector3 hitNormal, float damageAmount)
    {
        hp -= damageAmount; //hp에서 데미지 차감
        accumulatedDamage += damageAmount; //데미지 누적
        while (accumulatedDamage >= data.damagePer)
        {
            for (int i = 0; i < data.dropAmount; i++) //��� ������ ������ŭ �ݺ�
            { //������ ����
                Instantiate(data.dropItem, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
            }
            accumulatedDamage -= data.damagePer; //누적 데미지에서 드랍당 데미지 차감
            dropItem--; //드랍 가능 횟수 차감
            if (dropItem <= 0) //드랍 가능 횟수가 0이하이면
            {
                Instantiate(prefab, pos, rot); //원래 위치와 회전으로 프리팹 재생성
                Destroy(gameObject); //구조물 파괴
            }
        }
    }
   


}

