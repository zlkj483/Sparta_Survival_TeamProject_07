using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private DialogueLine[] line;

    public DialogueLine[] Line => line; // 외부 참조용(읽기전용)
}
