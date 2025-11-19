using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private DialogueLine[] lines;

    public DialogueLine[] Lines => lines; // 외부 참조용(읽기전용)
}
