using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine.Variables
{
    [System.Serializable]
    public abstract class MachineVariable
    {
        public abstract object GetVariable();
    }

    [System.Serializable]
    public class MachineVariable<T> : MachineVariable
    {
        public T Value;

        public override object GetVariable()
        {
            return Value;
        }
    }

    // =============================== GENERIC ===============================

    [System.Serializable] public class BoolVariable     : MachineVariable<bool>     { }
    [System.Serializable] public class IntVariable      : MachineVariable<int>      { }
    [System.Serializable] public class FloatVariable    : MachineVariable<float>    { }
    [System.Serializable] public class StringVariable   : MachineVariable<string>   { }
    [System.Serializable] public class Vector2Variable  : MachineVariable<Vector2>  { }
    [System.Serializable] public class Vector3Variable  : MachineVariable<Vector3>  { }

    // =============================== COMPONENTS ===============================

    [System.Serializable] public class MonoBehaviourVariable    : MachineVariable<MonoBehaviour>    { }
    [System.Serializable] public class TransformVariable        : MachineVariable<Transform>        { }
}