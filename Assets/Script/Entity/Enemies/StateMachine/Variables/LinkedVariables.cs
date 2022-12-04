using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine.Variables
{
    [System.Serializable]
    public class LinkedVariable<T>
    {
        [SerializeField] private string variableName;
        private MachineVariable<T> _value;

        public T Value { get => _value.Value; set => _value.Value = value; }

        public void Init(EnemyStateMachine machine)
        {
            _value = machine.GetMachineVariable<T>(variableName);
        }
        
    }
}