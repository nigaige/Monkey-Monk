using MonkeyMonk.Map;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[CustomEditor(typeof(Node), true)]
public class NodeEditor : Editor
{
    private Node _targetNode;
    private SerializedProperty _northProperty;
    private SerializedProperty _southProperty;
    private SerializedProperty _westProperty;
    private SerializedProperty _eastProperty;
    private SerializedProperty[] _pathsProperty;

    private void OnEnable()
    {
        _targetNode = target as Node;
        _northProperty = serializedObject.FindProperty("northPath");
        _southProperty = serializedObject.FindProperty("southPath");
        _westProperty = serializedObject.FindProperty("westPath");
        _eastProperty = serializedObject.FindProperty("eastPath");
        _pathsProperty = new SerializedProperty[] { _northProperty, _southProperty, _westProperty, _eastProperty };
    }

    public override void OnInspectorGUI()
    {
        //Object previousValue = _splineProperty.objectReferenceValue;

        // Make all the public and serialized fields visible in Inspector
        base.OnInspectorGUI();
        /*
        // Load changed values
        serializedObject.Update();

        // Check if value has changed
        if (previousValue != _splineProperty.objectReferenceValue)
        {
            Debug.Log("OK");
        }

        serializedObject.ApplyModifiedProperties();
        */

        foreach (SerializedProperty pathProperty in _pathsProperty)
        {
            if (pathProperty.objectReferenceValue == null) continue;

            NodePath path = pathProperty.objectReferenceValue as NodePath;

            if (path != null)
            {
                BezierKnot knot = path.GetTargetKnot(_targetNode);
                knot.Position = (float3)_targetNode.transform.position - (float3)path.transform.position;
                path.SetTargetKnot(_targetNode, knot);
            }
        }
    }
}
