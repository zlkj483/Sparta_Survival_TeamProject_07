using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIStateMachine
{
    private IAIState _current;

    public IAIState CurrentState => _current;

    public void ChangeState(IAIState next)
    {
        if (next == _current) return;

        _current?.OnExit();
        _current = next;
        _current?.OnEnter();
    }

    public void Tick()
    {
        _current?.Tick();
    }
}