using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class QuestManager : MonoBehaviour
{
    private static QuestManager _instance; // 실제 금고
    public static QuestManager Instance // 금고 열쇠(외부접근용)
    {
        get
        {
            if (_instance == null)
            {
                GameObject singletonObject = new GameObject("QuestManager"); // 순서를 지키자. 오브젝트 생성 후 컴퍼넌트 달아주기.
                _instance = singletonObject.AddComponent<QuestManager>();
                DontDestroyOnLoad(singletonObject);
                _instance.InitializeSingleton();
            }
            return _instance;
        }
    }
    private void InitializeSingleton() // 초기화 시 단 한 번만 로드되도록 방어 로직 추가
    {
        if (questDictionary.Count == 0)
        {
            LoadQuestData();
        }
    }
    [Header("모든 퀘스트 정보를 인스펙터에서 로드")]
    [SerializeField] private List<QuestData> allQuestData;
    private Dictionary<int, QuestInfo> questDictionary = new Dictionary<int, QuestInfo>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
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


