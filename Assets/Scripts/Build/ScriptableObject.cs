using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Game/Building Data")]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public Sprite icon;
    public string description;

    [System.Serializable]
    public class LevelInfo
    {
        public int requiredWood;
        public int requiredStone;
        public float buildTime;
        public GameObject prefab;   // 해당 단계의 실제 Prefab
    }

    public LevelInfo[] levels;
}
