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
    public void Gather(Vector3 hitPoint, Vector3 hitNormal, int damageAmount)
    {
        data.hp -= damageAmount;
        accumulatedDamage += damageAmount;
        while (accumulatedDamage >= data.damagePer)
        {
            for (int i = 0; i < data.dropAmount; i++)
            {
                Instantiate(data.dropItem, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
            }
            accumulatedDamage -= data.damagePer;
            data.dropCount--;
            if (data.dropCount <= 0)
            {
                Destroy(gameObject);
                StartCoroutine(Regen());
            }
        }
    }

    private IEnumerator Regen()
    {
        yield return new WaitForSeconds(30f);
        Instantiate(prefab,pos,rot);
    }
}

