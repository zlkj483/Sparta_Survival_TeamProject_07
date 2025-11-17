using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//대화 구조체

[Serializable]
public struct DialogueLine
{
    [SerializeField] private string _speakerName;
    [TextArea(3, 5)]
    [SerializeField] private string _text;

    public string SpeakerName => _speakerName; // 외부 참조용(읽기전용)
    public string Text => _text; // 외부 참조용(읽기전용)
}
