using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies
{
    public class EnemyStateMachine : MonoBehaviour
    {
        [SerializeField] private Enemy entity;
        public Enemy Entity { get => entity; }

        [SerializeField] private State rootState;

        private State _currentState;

        public void Awake()
        {
            // Init all states
            State[] states = gameObject.GetComponentsInChildren<State>();
            foreach (State state in states)
            {
                state.Initialize(this);
            }

            SwitchState(rootState);
        }

        public void SwitchState(State state)
        {
            _currentState?.ExitState();
            _currentState = state;
            _currentState?.EnterState();
        }

    }
}