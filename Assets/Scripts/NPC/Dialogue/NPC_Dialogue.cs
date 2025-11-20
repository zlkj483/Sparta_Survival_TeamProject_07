using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
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
}
