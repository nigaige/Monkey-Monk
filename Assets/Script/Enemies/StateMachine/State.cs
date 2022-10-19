using MonkeyMonk.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    private EnemyStateMachine _stateMachine;

    public bool IsCompleted { get; private set; }

    public Enemy Entity { get => _stateMachine.Entity; }

    public void Initialize(EnemyStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual void EnterState()
    {
        IsCompleted = false;
    }

    public virtual void UpdateState()
    {

    }

    public virtual void FixedUpdateState()
    {

    }

    public virtual void ExitState()
    {
        IsCompleted = true;
    }

}
