using MonkeyMonk.Enemies.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VariableDictVal))]
public class VariableDictValEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width / 2f, position.height), property.FindPropertyRelative("Name"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(position.x + position.width / 2f, position.y, position.width / 2f, position.height), property.FindPropertyRelative("Value"), GUIContent.none);
    }
}
