using MonkeyMonk.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine
{
    public abstract class State : MonoBehaviour
    {
        private EnemyStateMachine _stateMachine;

        public bool IsCompleted { get; private set; }

        public Enemy Entity { get => _stateMachine.Entity; }

        private State _currentSubState;


        public virtual void Initialize(EnemyStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }


        public virtual void EnterState()
        {
            IsCompleted = false;
        }

        public virtual void UpdateState()
        {
            _currentSubState?.UpdateState();
        }

        public virtual void FixedUpdateState()
        {
            _currentSubState?.FixedUpdateState();
        }

        public virtual void ExitState()
        {
            IsCompleted = true;
        }


        protected void SwitchSubState(State state)
        {
            _currentSubState?.ExitState();
            _currentSubState = state;
            _currentSubState?.EnterState();
        }

    }
}