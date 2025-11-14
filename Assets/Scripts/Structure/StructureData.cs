using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Structure", menuName = "New Structure")]

public class StructureData : ScriptableObject
{
        [Header("Info")]
        public string displayName;
        public GameObject dropPrefab;
    
}
