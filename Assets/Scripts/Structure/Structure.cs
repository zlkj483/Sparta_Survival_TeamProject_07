using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Structure : MonoBehaviour, IGatherable
{
    public StructureData data;
    private float accumulatedDamage;
    private float hp;
    private int dropItem;
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;
    private void Start()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        init();
    }
    private void init()
    {
        hp = data.maxHp;
        dropItem = data.dropCount;
        meshCollider.enabled = true; //콜라이더 활성화
        meshRenderer.enabled = true; //렌더러 활성화
        accumulatedDamage = 0;
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
                meshCollider.enabled = false; //콜라이더 비활성화
                meshRenderer.enabled = false; //렌더러 비활성화
                StartCoroutine(Regen()); //재생성 코루틴 시작

            }
        }
    }
    public IEnumerator Regen()
    {
        yield return new WaitForSeconds(30f); //30초 대기
        init();
    }



}

