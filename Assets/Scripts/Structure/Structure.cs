using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public StructureData data;
    private int accumulatedDamage = 0;
    public GameObject prefab;       // 재생성할 프리팹
    private Vector3 pos;    // 원래 위치
    private Quaternion rot; // 원래 회전

    private void Start()
    {
        pos = transform.position;
        rot = transform.rotation;
    }

                                //맞은위치 , 맞은법선 , 데미지량
    public void Gather(Vector3 hitPoint, Vector3 hitNormal, int damageAmount)
    {
        data.hp -= damageAmount; //hp에서 데미지 차감
        accumulatedDamage += damageAmount; //데미지 누적
        while (accumulatedDamage >= data.damagePer)
        {
            for (int i = 0; i < data.dropAmount; i++) //드랍 아이템 수량만큼 반복
            { //아이템 생성
                Instantiate(data.dropItem, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
            }
            accumulatedDamage -= data.damagePer; //누적 데미지에서 드랍당 데미지 차감
            data.dropCount--; //드랍 가능 횟수 차감
            if (data.dropCount <= 0) //드랍 가능 횟수가 0이하이면
            {
                Destroy(gameObject); //구조물 파괴
                StartCoroutine(Regen()); //재생성 코루틴 시작
            }
        }
    }

    private IEnumerator Regen()
    {
        yield return new WaitForSeconds(30f); //30초 대기
        Instantiate(prefab,pos,rot); //원래 위치와 회전으로 프리팹 재생성
    }
}

