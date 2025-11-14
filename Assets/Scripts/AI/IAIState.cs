using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAIState
{
    void OnEnter();
    void Tick();
    void OnExit();
}