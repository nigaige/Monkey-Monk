#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonkeyMonk.Enemies.StateMachine.Variables
{
    [CustomPropertyDrawer(typeof(MachineVariable), true)]
    public class MachineVariableEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.FindPropertyRelative("Value") != null) EditorGUI.PropertyField(position, property.FindPropertyRelative("Value"), GUIContent.none);
        }
    }
}
#endif