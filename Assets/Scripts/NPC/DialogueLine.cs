using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//대화 구조체

[Serializable]
public struct DialogueLine
{
    [SerializeField] private string speakerName;
    [TextArea(3, 5)]
    [SerializeField] private string text;

    public string SpeakerName => speakerName; // 외부 참조용(읽기전용)
    public string Text => text; // 외부 참조용(읽기전용)
}
