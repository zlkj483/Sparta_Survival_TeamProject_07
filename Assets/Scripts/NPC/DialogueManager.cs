using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
public static DialogueManager Instance { get; private set; }
    [Header("Dialogue Components")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _speakerNameText;
    [SerializeField] private TextMeshProUGUI _dialogueContentText;

    [Header("Settings")]
    [SerializeField] private float _typingSpeed = 0.05f;

    private Queue<DialogueLine> _dialogueQueue;
    private Coroutine _dialogueRoutine;
    private bool _isTyping = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            _dialogueQueue = new Queue<DialogueLine>();
            _dialoguePanel.SetActive(false);
        }
    }

    public void StartDialogue(DialogueData data) //NPC가 대화를 시작할 때 호출하는 진입점
    {
        if (_dialoguePanel.activeSelf) return; // 이미 대화중이면 무시

        _dialogueQueue.Clear();
        foreach(DialogueLine line in data.Line) // 큐 초기화 및 데이터 로드
        {
            _dialogueQueue.Enqueue(line);
        }
        _dialoguePanel.SetActive(true);
        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if(_dialogueQueue.Count == 0)
        {
        EndDialogue();
            return;
        }
        DialogueLine line = _dialogueQueue.Dequeue();
        _speakerNameText.text = line.SpeakerName;

        if(_dialogueRoutine != null) StopCoroutine(_dialogueRoutine);
        _dialogueRoutine = StartCoroutine(TypeSentence(line.Text));

    }

    private IEnumerator TypeSentence(string sentance)  // 타이핑 루틴
    {
        _isTyping = true;
        _dialogueContentText.text = "";
        foreach(char letter  in sentance.ToCharArray())
        {
            _dialogueContentText.text += letter;
            yield return new WaitForSeconds(_typingSpeed);
        }
        _isTyping = false;
    }

    private void EndDialogue()
    {
        _dialoguePanel.SetActive(false);
        Debug.Log("End Dialogue");
    }

    public void OnContinueClicked()  // 플레이어가 클릭하면 다음 대화 호출
    {
        if (_isTyping)
        {
            StopCoroutine(_dialogueRoutine);
            DialogueLine currentLine = _dialogueQueue.Dequeue();
            _dialogueContentText.text = _dialogueQueue.Peek().Text;
            _isTyping =false;
        }
        else
        {
            DisplayNextLine();
        }
    }
}
