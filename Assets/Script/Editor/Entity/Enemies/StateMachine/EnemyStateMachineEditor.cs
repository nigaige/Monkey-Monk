#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MonkeyMonk.Enemies.StateMachine.Variables;
using UnityEditorInternal;
using System;

namespace MonkeyMonk.Enemies.StateMachine
{
    [CustomEditor(typeof(EnemyStateMachine))]
    public class EnemyStateMachineEditor : Editor
    {
        private EnemyStateMachine _enemyStateMachine;
        private SerializedProperty _variablesProperty;

        private ReorderableList _variablesList;

        private void OnEnable()
        {
            _enemyStateMachine = (EnemyStateMachine)target;
            _variablesProperty = serializedObject.FindProperty("variables");

            _variablesList = new ReorderableList(serializedObject, _variablesProperty, true, false, true, true);

            _variablesList.drawElementCallback = DrawListItems;
            _variablesList.onAddDropdownCallback = OnAddDropdown;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            _variablesList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _variablesList.serializedProperty.GetArrayElementAtIndex(index);

            EditorGUI.PropertyField(rect, element, GUIContent.none);
        }

        void OnAddDropdown(Rect rect, ReorderableList list)
        {
            var menu = new GenericMenu();

            foreach (var item in s_MachineVariablesHierarchy.Keys)
            {
                menu.AddItem(new GUIContent(item), false, AddVariableToList, item);
            }

            menu.ShowAsContext();
        }

        void AddVariableToList(object param)
        {
            string val = (string)param;

            _variablesList.serializedProperty.arraySize++;
            var elem = _variablesList.serializedProperty.GetArrayElementAtIndex(_variablesList.serializedProperty.arraySize - 1);

            elem.FindPropertyRelative("Name").stringValue = "variable";
            elem.FindPropertyRelative("Value").managedReferenceValue = s_MachineVariablesHierarchy[val].Invoke();

            serializedObject.ApplyModifiedProperties();
        }

        private static readonly Dictionary<string, Func<MachineVariable>> s_MachineVariablesHierarchy = new Dictionary<string, Func<MachineVariable>>()
        {
            {"Generic/Bool", () => new BoolVariable() },
            {"Generic/Int", () => new IntVariable() },
            {"Generic/Float", () => new FloatVariable() },
            {"Generic/String", () => new StringVariable() },
            {"Generic/Vector2", () => new Vector2Variable() },
            {"Generic/Vector3", () => new Vector3Variable() },

            {"Behaviours/MonoBehaviour", () => new MonoBehaviourVariable() },
            {"Behaviours/Transform", () => new TransformVariable() }
        };

    }
}



#endif