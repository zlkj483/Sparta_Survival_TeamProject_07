using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class QuestManager : MonoBehaviour
{
    //private static QuestManager Instance; // 실제 금고
    public static QuestManager Instance { get; private set; }
    

    [Header("모든 퀘스트 정보를 인스펙터에서 로드")]
    [SerializeField] private List<QuestData> allQuestData;
    private Dictionary<int, QuestInfo> questDictionary = new Dictionary<int, QuestInfo>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadQuestData();
    }
    public bool CheckQuestCompletion(int questID) // 완료체크
    {
        QuestInfo quest = GetQuest(questID);
        if (quest == null || quest.state != QuestState.ONGOING) return false;

        bool allGoalsComplete = true;
        foreach (var goal in quest.goals)
        {
            if (!goal.IsComplete)
            {
                allGoalsComplete = false;
                break;
            }
        }

        if (allGoalsComplete)
        {
            quest.state = QuestState.CLEAR;

        }

        return allGoalsComplete;
    }

    private void LoadQuestData() // 퀘스트 데이터를 Dictionary로 변환하여 로드
    {
        if (allQuestData == null)
        {
            Debug.LogWarning("QuestManager: allQuestData가 null입니다.");
            return;
        }
        questDictionary.Clear();
        if (allQuestData.Count == 0)
        {
            Debug.Log("QuestManager: 로드할 퀘스트 데이터가 없습니다.");
            return;
        }
        foreach (var questData in allQuestData)
        {
            QuestInfo newQuestInstance = new QuestInfo(questData);

            if (!questDictionary.ContainsKey(newQuestInstance.QuestID))
            {
                newQuestInstance.state = QuestState.NEVER_RECEIVED;
                questDictionary.Add(newQuestInstance.QuestID, newQuestInstance);
            }
            else
            {
                Debug.LogError($"[QuestManager] 중복된 QuestID가 발견되었습니다: {newQuestInstance.QuestID}");
            }

        }
        Debug.Log($"[QuestManager] {questDictionary.Count}개의 퀘스트 데이터 로드 완료.");
    }

    public QuestInfo GetQuest(int questID) // npc 상호작용 시 필요한 퀘스트 id를 가져옴
    {
        if (questDictionary.ContainsKey(questID))
        {
            return questDictionary[questID];
        }
        return null;
    }

    public void StartQuest(int questID)
    {
        QuestInfo quest = GetQuest(questID);
        if (quest == null || quest.state != QuestState.NEVER_RECEIVED) return;

        quest.state = QuestState.ONGOING;
        Debug.Log($"[Quest] 퀘스트 시작: {quest.QuestName}");
        CheckInitialInventory(quest); // 퀘스트 시작 시 보유 아이템 확인
    }


    public void CompleteQuest(int questID) // 퀘스트 클리어 달성하면
    {
        QuestInfo quest = GetQuest(questID);
        if (quest == null || quest.state != QuestState.CLEAR) // 퀘스트 없거나 클리어 미달성
        {
            Debug.Log($"Quest {quest.QuestName}은 완료 준비가 되지 않았습니다.");
            return;
        }
        quest.state = QuestState.CLEARED_PAST; // 퀘스트 상태 클리어로 바꿔주고 완료 로그
        Debug.Log($"[Quest] 퀘스트 {quest.QuestName} 완료");
    }
    private void CheckInitialInventory(QuestInfo quest)
    {
        UIInventory invManager = UIInventory.Instance;

        if( invManager == null )
        {
            Debug.LogError("퀘스트매니저에서 UI인벤토리 불러오기 실패");
            return;
        }
        foreach(var goal in quest.goals)
        {
            if(goal.goalType == QuestGoalType.Gather)
            {
                // invManager를 통해 보유 아이템 가져오기
                int currentInventoryCount = invManager.QuestItemCount(goal.targetID);

                if( currentInventoryCount > 0) // 보유 아이템 있을 때
                {
                    int amountToSet = Mathf.Min(currentInventoryCount, goal.requiredAmount);
                    goal.currentAmount = amountToSet;
                    Debug.Log($"초기 보유량 반영: {quest.QuestName} - {goal.targetID} {goal.currentAmount}/{goal.requiredAmount}");
                    
                }
            }
            
        }
        if (QuestManager.Instance != null) // 초기 보유량으로 퀘스트 완료되었는지 확인
        {
            QuestManager.Instance.CheckQuestCompletion(quest.QuestID);
        }
    }

    public void UpdateQuestGoal(QuestGoalType goalType, string targetID, int amount = 1) // 골타입, 아이템id, 증가량 1씩
    {
        foreach (var kvp in questDictionary)
        {
            QuestInfo quest = kvp.Value;
            if (quest.state == QuestState.ONGOING)
            {
                foreach (var goal in quest.goals)
                {
                    if (goal.goalType == goalType && goal.targetID == targetID && !goal.IsComplete)
                    {
                        goal.currentAmount += amount;

                        if (goal.currentAmount > goal.requiredAmount) // 진행도가 요구량을 초과하지 않게
                        {
                            goal.currentAmount = goal.requiredAmount;
                        }
                        Debug.Log($"목표 진행: {quest.QuestName} - {goal.targetID} {goal.currentAmount}/{goal.requiredAmount}");
                        CheckQuestCompletion(quest.QuestID);//퀘스트 완료 여부 확인
                        return;
                    }
                }
            }
        }
    }

}


