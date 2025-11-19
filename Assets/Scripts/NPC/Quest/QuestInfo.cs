using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public enum QuestGoalType { Gather }  // 퀘스트 목표 타입. 추후 추가 및 삭제 가능.

/*public enum QuestState  // 퀘스트 상태
{
    NEVER_RECEIVED,
    ONGOING,
    CLEAR,
    CLEARED_PAST
}
/*[System.Serializable] // inspector창에서 변수 노출
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
    public int rewardItem;
    [Header("퀘스트 NPC")]
    public int[] questNPCID;
    public QuestState state = QuestState.NEVER_RECEIVED;
    public QuestInfo(QuestData data)
    {
        this.QuestID = data.QuestID;
        this.QuestName = data.QuestName;
        this.QuestDescription = data.QuestDescription;
        this.rewardItem = data.rewardItem;
        this.questNPCID = data.questNPCID;

        this.goals = new List<QuestGoal>();
        if (data.goals != null)
        {
            foreach (var goal in data.goals)
            {
                this.goals.Add(new QuestGoal(goal));
            }
        }

        this.state = QuestState.NEVER_RECEIVED;
    }

    public QuestInfo() { }
}*/




