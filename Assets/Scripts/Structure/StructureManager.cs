using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class StructureManager : MonoBehaviour
{
    protected Vector3 pos;    // 원래 위치
    protected Quaternion rot; // 원래 회전
    public GameObject prefab;       // 재생성할 프리팹
    private void Start()
    {
        pos = transform.position;
        rot = transform.rotation;
        StartCoroutine(Regen()); //재생성 코루틴 시작
    }

    protected IEnumerator Regen()
    {

        yield return new WaitForSeconds(30f); //30초 대기
        Instantiate(prefab, pos, rot); //원래 위치와 회전으로 프리팹 재생성


    }
}
