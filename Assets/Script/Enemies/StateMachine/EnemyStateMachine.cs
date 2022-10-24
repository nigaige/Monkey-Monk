using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine
{
    public class EnemyStateMachine : MonoBehaviour
    {
        [SerializeField] private Enemy entity;
        public Enemy Entity { get => entity; }

        [SerializeField] private State startingState;

        private State _currentState;

        public void Awake()
        {
            // Init all states
            State[] states = gameObject.GetComponentsInChildren<State>();
            foreach (State state in states)
            {
                state.Initialize(this);
            }

            SwitchState(startingState);
        }

        public void SwitchState(State state)
        {
            _currentState?.ExitState();
            _currentState = state;
            _currentState?.EnterState();
        }

        private void Update()
        {
            _currentState?.UpdateState();
        }

        private void FixedUpdate()
        {
            _currentState?.FixedUpdateState();
        }

    }
}