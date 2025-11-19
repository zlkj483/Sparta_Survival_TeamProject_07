using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum QuestGoalType { Kill, Gather, Interact }  // 퀘스트 목표 타입. 추후 추가 및 삭제 가능.

public enum QuestState  // 퀘스트 상태
{
    NEVER_RECEIVED,
    ONGOING,
    CLEAR,
    CLEARED_PAST
}
[System.Serializable] // inspector창에서 변수 노출
public class QuestGoal // 퀘스트의 개별 목표
{
    public QuestGoalType goalType; // 목표 타입
    public string targetID; // 목표 대상
    public int requiredAmount; // 목표치

    public int currentAmount; // 현재진행도

    public bool IsComplete => currentAmount >= requiredAmount; // 달성여부
}


[System.Serializable]
public class QuestInfo //퀘스트 정보
{
    public int QuestID;
    public string QuestName;

    [UnityEngine.TextArea]
    public string QuestDescription;

    public List<QuestGoal> goals;
    public QuestState state = QuestState.NEVER_RECEIVED;
    public int rewardItem; // 보상을 어떻게할까요?

    [Header("퀘스트 NPC")]
    public int[] questNPCID;

}


