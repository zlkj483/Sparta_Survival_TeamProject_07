
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
                // 일반 대화 컴포넌트의 OnInteract 함수를 호출합니다.
                npcDialogue.OnInteract();
            }
        }
    }

    // 플레이어가 NPC의 트리거(Box Collider) 범위에 진입했을 때
    private void OnTriggerEnter(Collider other)
    {
        // 🛑 플레이어 캐릭터에 "Player" 태그가 붙어있어야 작동합니다.
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 상호작용 힌트 출력 (선택적)
            Debug.Log(npcDialogue != null ? npcDialogue.GetInteractPrompt() : "NPC와 상호작용 가능");
        }
    }

    // 플레이어가 범위에서 벗어났을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}