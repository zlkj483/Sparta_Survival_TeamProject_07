using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class StructureManager : MonoBehaviour
{
    public static StructureManager instance;

    public IEnumerator Regen(GameObject prefab , Vector3 pos ,Quaternion rot)
    {
        yield return new WaitForSeconds(30f); //30초 대기
        Instantiate(prefab, pos, rot); //원래 위치와 회전으로 프리팹 재생성
    }
}
