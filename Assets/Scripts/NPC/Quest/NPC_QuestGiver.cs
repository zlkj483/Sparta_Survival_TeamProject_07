using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_QuestGiver : MonoBehaviour
{
    [Header("NPC 고유 정보")]
    public int npcID = 1;
    [Header("이 NPC가 부여할 퀘스트")]
    [SerializeField] private int questIDToGive;
    [SerializeField] private DialogueData dialogue_startQuest; // 퀘스트 시작 대화
    [SerializeField] private DialogueData dialogue_ongoing; // 퀘스트 진행중 대화
    [SerializeField] private DialogueData dialogue_readyToComplete; // 퀘스트 완료 대화

    public void Interact()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogError("퀘스트매니저 연결안됨");
            return;
        }
        QuestInfo quest = QuestManager.Instance.GetQuest(questIDToGive);
        if (quest == null)
        {
            Debug.LogError($"NPC {npcID}: 부여할 퀘스트 (ID: {questIDToGive}) 정보를 찾을 수 없습니다.");
            return;
        }
        switch (quest.state)
        {
            case QuestState.NEVER_RECEIVED:
                DialogueManager.Instance.StartDialogue(dialogue_startQuest);
                QuestManager.Instance.StartQuest(questIDToGive);
                Debug.Log($"NPC {npcID}: 플레이어에게 퀘스트 '{quest.QuestName}'를 부여했습니다.");
                break;

            case QuestState.ONGOING:
                DialogueManager.Instance.StartDialogue(dialogue_ongoing);
                Debug.Log($"NPC {npcID}: 퀘스트 '{quest.QuestName}'는 진행중입니다.");
                break;

            case QuestState.CLEAR:
                DialogueManager.Instance.StartDialogue(dialogue_readyToComplete);
                QuestManager.Instance.CompleteQuest(questIDToGive);
                Debug.Log($"NPC {npcID}: 퀘스트 '{quest.QuestName}' 완료");
                break;
            case QuestState.CLEARED_PAST :
                Debug.Log("이미 이 퀘스트는 완료되었습니다");
                break;

        }
    }
}
