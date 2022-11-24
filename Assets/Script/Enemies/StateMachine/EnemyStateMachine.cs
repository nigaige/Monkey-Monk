using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine
{
    public class EnemyStateMachine : MonoBehaviour
    {
        public Enemy Entity { get; private set; }

        [SerializeField] private State startingState;
        [SerializeField] private State knockState;

        private State _currentState;

        public void Initialize(Enemy entity)
        {
            Entity = entity;

            // Init all states
            State[] states = gameObject.GetComponentsInChildren<State>();
            foreach (State state in states)
            {
                state.Initialize(this);
            }

            SwitchState(startingState);
        }

        public void Knock()
        {
            SwitchState(knockState);
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

            if (_currentState != null && _currentState.IsCompleted)
            {
                SwitchState(startingState);
            }
        }

        private void FixedUpdate()
        {
            _currentState?.FixedUpdateState();
        }

    }
}