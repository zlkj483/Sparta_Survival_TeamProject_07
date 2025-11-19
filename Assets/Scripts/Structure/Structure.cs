using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public StructureData data;
    private int accumulatedDamage = 0;
    public GameObject prefab;       // ������� ������
    private Vector3 pos;    // ���� ��ġ
    private Quaternion rot; // ���� ȸ��

    private void Start()
    {
        pos = transform.position;
        rot = transform.rotation;
    }

    //������ġ , �������� , ��������
    public void Gather(Vector3 hitPoint, Vector3 hitNormal, int damageAmount)
    {
        data.hp -= damageAmount; //hp���� ������ ����
        accumulatedDamage += damageAmount; //������ ����
        while (accumulatedDamage >= data.damagePer)
        {
            for (int i = 0; i < data.dropAmount; i++) //��� ������ ������ŭ �ݺ�
            { //������ ����
                Instantiate(data.dropItem, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal, Vector3.up));
            }
            accumulatedDamage -= data.damagePer; //���� ���������� ����� ������ ����
            data.dropCount--; //��� ���� Ƚ�� ����
            if (data.dropCount <= 0) //��� ���� Ƚ���� 0�����̸�
            {
                Destroy(gameObject); //������ �ı�
                StartCoroutine(Regen()); //����� �ڷ�ƾ ����
            }
        }
    }

    private IEnumerator Regen()
    {
        yield return new WaitForSeconds(30f); //30�� ���
        Instantiate(prefab, pos, rot); //���� ��ġ�� ȸ������ ������ �����
    }
}

