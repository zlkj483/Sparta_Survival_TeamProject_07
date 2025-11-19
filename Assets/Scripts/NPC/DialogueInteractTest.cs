
using UnityEngine;

public class DialogueInteraction : MonoBehaviour
{
    private NPC_Dialogue npcDialogue; // 일반 대화 컴포넌트
    private bool isPlayerInRange = false;

    private void Start()
    {
        npcDialogue = GetComponent<NPC_Dialogue>();

        if (npcDialogue == null)
        {
            Debug.LogError(gameObject.name + ": NPC_Dialogue 컴포넌트를 찾을 수 없습니다. 상호작용 테스트 불가.");
        }
    }

    private void Update()
    {
        
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (npcDialogue != null)
            {
                npcDialogue.OnInteract();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log(npcDialogue != null ? npcDialogue.GetInteractPrompt() : "NPC와 상호작용 가능");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}