using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;


namespace MonkeyMonk.Inputs
{
    [CustomEditor(typeof(InputLinker))]
    public class InputLinkerEditor : Editor
    {
        private SerializedProperty _actionsProperty;
        private SerializedProperty _actionEventsProperty;

        [SerializeField] private bool eventsGroupUnfolded;
        [SerializeField] private bool[] _actionMapEventsUnfolded;

        private GUIContent[] _actionNames = null;
        private int _numActionMaps;
        private GUIContent[] _actionMapNames;
        private int[] _actionMapIndices;

        private bool _actionAssetInitialized = false;


        private void OnEnable()
        {
            _actionsProperty = serializedObject.FindProperty("actions");
            _actionEventsProperty = serializedObject.FindProperty("actionEvents");
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_actionsProperty);
            if (EditorGUI.EndChangeCheck() || !_actionAssetInitialized)
            {
                OnActionAssetChange();
            }

            eventsGroupUnfolded = EditorGUILayout.Foldout(eventsGroupUnfolded, EditorGUIUtility.TrTextContent("Events", "UnityEvents triggered by the PlayerInput component"), toggleOnLabelClick: true);
            if (eventsGroupUnfolded)
            {
                // Action events. Group by action map.
                if (_actionNames != null)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        for (var n = 0; n < _numActionMaps; ++n)
                        {
                            // Skip action maps that have no names (case 1317735).
                            if (_actionMapNames[n] == null)
                                continue;

                            _actionMapEventsUnfolded[n] = EditorGUILayout.Foldout(_actionMapEventsUnfolded[n],
                                _actionMapNames[n], toggleOnLabelClick: true);
                            using (new EditorGUI.IndentLevelScope())
                            {
                                if (_actionMapEventsUnfolded[n])
                                {
                                    for (var i = 0; i < _actionNames.Length; ++i)
                                    {
                                        if (_actionMapIndices[i] != n)
                                            continue;

                                        EditorGUILayout.PropertyField(_actionEventsProperty.GetArrayElementAtIndex(i), _actionNames[i]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }

        private void OnActionAssetChange()
        {
            serializedObject.ApplyModifiedProperties();
            _actionAssetInitialized = true;

            var inputLinker = (InputLinker)target;
            var asset = (InputActionAsset)_actionsProperty.objectReferenceValue;
            if (asset == null)
            {
                _actionNames = null;
                return;
            }

            // ===

            var newActionNames = new List<GUIContent>();
            var newActionEvents = new List<PlayerInput.ActionEvent>();
            var newActionMapIndices = new List<int>();

            _numActionMaps = 0;
            _actionMapNames = null;

            int LengthSafe<TValue>(TValue[] array)
            {
                if (array == null)
                    return 0;
                return array.Length;
            }

            void AddEntry(InputAction action, PlayerInput.ActionEvent actionEvent)
            {
                newActionNames.Add(new GUIContent(action.name));
                newActionEvents.Add(actionEvent);

                var actionMapIndex = asset.actionMaps.IndexOfReference(action.actionMap);
                newActionMapIndices.Add(actionMapIndex);

                if (actionMapIndex >= _numActionMaps)
                    _numActionMaps = actionMapIndex + 1;

                if (LengthSafe(_actionMapNames) < actionMapIndex + 1)
                    Array.Resize(ref _actionMapNames, actionMapIndex + 1);

                if (EqualityComparer<GUIContent>.Default.Equals(_actionMapNames[actionMapIndex], default(GUIContent)))
                    _actionMapNames[actionMapIndex] = new GUIContent(action.actionMap.name);

            }

            // Bring over any action events that we already have and that are still in the asset.
            PlayerInput.ActionEvent[] oldActionEvents = inputLinker.actionEvents;
            if (oldActionEvents != null)
            {
                foreach (var entry in oldActionEvents)
                {
                    var guid = entry.actionId;
                    var action = asset.FindAction(guid);
                    if (action != null)
                        AddEntry(action, entry);
                }
            }

            // Add any new actions.
            foreach (var action in asset)
            {
                // Skip if it was already in there.
                if (oldActionEvents != null && oldActionEvents.Any(x => x.actionId == action.id.ToString()))
                    continue;

                ////FIXME: adds bindings to the name
                AddEntry(action, new PlayerInput.ActionEvent(action.id, action.ToString()));
            }

            _actionNames = newActionNames.ToArray();
            _actionMapIndices = newActionMapIndices.ToArray();
            Array.Resize(ref _actionMapEventsUnfolded, _numActionMaps);
            inputLinker.actionEvents = newActionEvents.ToArray();

            serializedObject.Update();
        }
    }
}