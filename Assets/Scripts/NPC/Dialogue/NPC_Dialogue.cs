using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class NPC_Dialogue : MonoBehaviour
{
    [Header("상호작용 UI")]
    [SerializeField] private GameObject interactionPromptUI; // 텍스트 연결할곳
    private bool isPlayerInRange = false; // 플레이어 감지용

    [SerializeField] private DialogueData dialogueData;

    public string GetInteractPrompt() // 나중에 키 입력 따라 수정 가능.
    {
        return $"[E] {gameObject.name} 와(과) 대화";
    }

    public void OnInteract()
    {
        if(dialogueData != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueData);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(false);
            }
        }
    }
}
