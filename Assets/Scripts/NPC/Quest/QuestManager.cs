using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    [Header("모든 퀘스트 정보를 인스펙터에서 로드")]
    [SerializeField] private List<QuestInfo> allQuestData;
    private Dictionary<int, QuestInfo> questDictionary = new Dictionary<int, QuestInfo>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadQuestData();
    }

    private void LoadQuestData() // 퀘스트 데이터를 Dictionary로 변환하여 로드
    {
        questDictionary.Clear();
        foreach (var quest in allQuestData)
        {
            if(!questDictionary.ContainsKey(quest.QuestID))
            {
                quest.state = QuestState.NEVER_RECEIVED;
                questDictionary.Add(quest.QuestID, quest);
            }
            else
            {
                Debug.LogError($"[QuestManager] 중복된 QuestID가 발견되었습니다: {quest.QuestID}");
            }
            
        }
        Debug.Log($"[QuestManager] {questDictionary.Count}개의 퀘스트 데이터 로드 완료.");
    }

    public QuestInfo GetQuest(int questID) // npc 상호작용 시 필요한 퀘스트 id를 가져옴
    {
        if( questDictionary.ContainsKey(questID))
        {
            return questDictionary[questID];
        }
        return null;
    }

    public void StartQuest(int questID)
    {
        QuestInfo quest = GetQuest(questID);
        if(quest == null || quest.state != QuestState.NEVER_RECEIVED) return;

        quest.state = QuestState.ONGOING;
        Debug.Log($"[Quest] 퀘스트 시작: {quest.QuestName}");
    }

    public void CompleteQuest(int questID) // 퀘스트 클리어 달성하면
    {
        QuestInfo quest = GetQuest(questID);
        if(quest == null || quest.state != QuestState.CLEAR) // 퀘스트 없거나 클리어 미달성
        {
            Debug.Log($"Quest {quest.QuestName}은 완료 준비가 되지 않았습니다.");
            return;
        }
        quest.state = QuestState.CLEARED_PAST; // 퀘스트 상태 클리어로 바꿔주고 완료 로그
        Debug.Log($"[Quest] 퀘스트 {quest.QuestName} 완료");
    }
}


