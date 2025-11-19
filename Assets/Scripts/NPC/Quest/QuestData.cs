using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewQuestData", menuName = "Quest/Quest Data", order = 1)]

public class QuestData : ScriptableObject
{
    [Header("기본 정보")]
    public int QuestID;
    public string QuestName;
    [UnityEngine.TextArea]
    public string QuestDescription;

    [Header("목표 및 보상")]
    // QuestGoal 정의가 있는 파일에 접근 가능해야 합니다.
    public List<QuestGoal> goals;
    public int rewardItem;

    [Header("퀘스트 연관 NPC")]
    public int[] questNPCID;
}
