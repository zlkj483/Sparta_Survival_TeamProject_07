using UnityEngine;
using System.Collections.Generic;
using System;

// 퀘스트 목표 타입
public enum QuestGoalType { Gather }

// 퀘스트 상태
public enum QuestState
{
    NEVER_RECEIVED,
    ONGOING,
    CLEAR,
    CLEARED_PAST
}

[System.Serializable]
public class QuestGoal // 퀘스트의 개별 목표 (깊은 복사 로직 포함)
{
    // 정적 필드
    public QuestGoalType goalType;
    public string targetID;
    public int requiredAmount;

    // 런타임 필드
    public int currentAmount;

    // 편의 속성
    public bool IsComplete => currentAmount >= requiredAmount;

    // 복사 생성자
    public QuestGoal(QuestGoal template)
    {
        this.goalType = template.goalType;
        this.targetID = template.targetID;
        this.requiredAmount = template.requiredAmount;
        this.currentAmount = 0; // 런타임 데이터 초기화
    }

    public QuestGoal() { } // 기본 생성자
}

[System.Serializable]
public class QuestInfo // 퀘스트 정보 (런타임 상태 관리)
{
    // QuestData와 동일한 필드들
    public int QuestID;
    public string QuestName;
    [UnityEngine.TextArea]
    public string QuestDescription;
    public List<QuestGoal> goals;
    public int rewardItem;
    [Header("퀘스트 NPC")]
    public int[] questNPCID;

    // 런타임 상태 필드
    public QuestState state = QuestState.NEVER_RECEIVED;

    // QuestData를 받아 복사하는 생성자
    public QuestInfo(QuestData data)
    {
        // 1. 단순 값 복사
        this.QuestID = data.QuestID;
        this.QuestName = data.QuestName;
        this.QuestDescription = data.QuestDescription;
        this.rewardItem = data.rewardItem;
        this.questNPCID = data.questNPCID;

        // 2. 깊은 복사: QuestGoal 리스트 초기화 및 복사
        this.goals = new List<QuestGoal>();
        if (data.goals != null)
        {
            foreach (var goal in data.goals)
            {
                this.goals.Add(new QuestGoal(goal));
            }
        }

        // 3. 런타임 상태 초기화
        this.state = QuestState.NEVER_RECEIVED;
    }

    public QuestInfo() { } // 기본 생성자
}