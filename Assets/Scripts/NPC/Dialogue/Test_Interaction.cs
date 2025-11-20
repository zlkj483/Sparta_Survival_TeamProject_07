using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Cube_Interaction : MonoBehaviour
{
    private NPC_QuestGiver questGiver;

    private bool isPlayerInRange = false;

    [Header("상호작용 설정")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    private const string PlayerTag = "Player";

    void Start()
    {
        questGiver = GetComponent<NPC_QuestGiver>();
        if (questGiver == null)
        {
            Debug.LogError("Cube_Interaction: NPC_QuestGiver 스크립트를 찾을 수 없습니다! NPC 큐브에 QuestGiver를 붙여주세요.");
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            if (questGiver != null)
            {
                questGiver.StartQuestInteract();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            isPlayerInRange = true;
            Debug.Log("E키를 눌러 큐브와 대화하세요.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            isPlayerInRange = false;
            Debug.Log("큐브로부터 멀어졌습니다.");
        }
    }
}