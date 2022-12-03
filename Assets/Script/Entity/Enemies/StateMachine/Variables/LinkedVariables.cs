using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine.Variables
{
    [System.Serializable]
    public class LinkedVariable<T>
    {
        [SerializeField] private string variableName;
        [SerializeField] [HideInInspector] private T value;

        public T Value { get => value; }

        public void Init(EnemyStateMachine machine)
        {
            value = machine.GetVariable<T>(variableName);
        }
        
    }
}