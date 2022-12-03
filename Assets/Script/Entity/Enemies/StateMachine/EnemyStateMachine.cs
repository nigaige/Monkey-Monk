using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonkeyMonk.Enemies.StateMachine.Variables;

namespace MonkeyMonk.Enemies.StateMachine
{
    public class EnemyStateMachine : MonoBehaviour
    {
        public Enemy Entity { get; private set; }

        [SerializeField] private State startingState;
        [SerializeField] private State knockState;

        [SerializeField] [HideInInspector] private List<VariableDictVal> variables;
        private Dictionary<string, MachineVariable> _variablesDict;

        private State _currentState;

        public void Initialize(Enemy entity)
        {
            Entity = entity;

            _variablesDict = new();

            foreach (var item in variables)
            {
                _variablesDict.Add(item.Name, item.Value);
            }

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

        public void AddVariable(string variableName, MachineVariable variable)
        {
            variables.Add(new VariableDictVal(variableName, variable));
        }

        public T GetVariable<T>(string variableName)
        {
            if (_variablesDict.TryGetValue(variableName, out MachineVariable value))
            {
                return (T)value.GetVariable();
            }

            return default;
        }

        public MachineVariable GetMachineVariable(int i)
        {
            return variables[i].Value;
        }

    }

    [System.Serializable]
    public class VariableDictVal
    {
        public string Name;
        [SerializeReference] public MachineVariable Value;

        public VariableDictVal(string variableName, MachineVariable value)
        {
            this.Name = variableName;
            this.Value = value;
        }
    }
}