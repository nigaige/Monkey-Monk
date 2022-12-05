#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EntityWalkableZone))]
public class EntityWalkableZoneEditor : Editor
{
    EntityWalkableZone _zone;

    private SerializedProperty _startPointProperty;
    private SerializedProperty _endPointProperty;

    public bool IsInEditMode
    {
        get => _isInEditMode;
        set
        {
            if (value)
            {
                _lastTool = Tools.current;
                Tools.current = Tool.None;
            }
            else if(Tools.current == Tool.None)
            {
                Tools.current = _lastTool;
            }
            _isInEditMode = value; 
        }
    }
    private bool _isInEditMode;
    private Tool _lastTool = Tool.None;

    private void OnEnable()
    {
        _zone = (EntityWalkableZone)target;
        _startPointProperty = serializedObject.FindProperty("_startZoneOffset");
        _endPointProperty = serializedObject.FindProperty("_endZoneOffset");

        _lastTool = Tools.current;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!IsInEditMode)
        {
            if (GUILayout.Button("Edit Zone Positions"))
            {
                IsInEditMode = true;
                SceneView.RepaintAll();
            }
        }
        else
        {
            if (GUILayout.Button("Disable Edit"))
            {
                IsInEditMode = false;
                SceneView.RepaintAll(); ;
            }
        }
    }

    void OnSceneGUI()
    {
        if (IsInEditMode)
        {
            Vector3 newStartOffset = Handles.PositionHandle(_zone.transform.position + _startPointProperty.vector3Value, Quaternion.identity) - _zone.transform.position;
            if (_startPointProperty.vector3Value != newStartOffset) _startPointProperty.vector3Value = newStartOffset;

            Vector3 newEndOffset = Handles.PositionHandle(_zone.transform.position + _endPointProperty.vector3Value, Quaternion.identity) - _zone.transform.position;
            if (_endPointProperty.vector3Value != newStartOffset) _endPointProperty.vector3Value = newEndOffset;

            serializedObject.ApplyModifiedProperties();
        }
    }

    private void OnDisable()
    {
        IsInEditMode = false;
        SceneView.RepaintAll();
    }

}
#endif