using Codice.CM.Common;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(RepeatNode))]
public class RepeatNodeEditor : Editor
{
    SerializedProperty _repeatTypeProp;
    SerializedProperty _countProp;
    SerializedProperty _durationProp;

    private void OnEnable()
    {
        if (target == null)
            return;

        _repeatTypeProp = serializedObject.FindProperty("_repeatType");
        _countProp = serializedObject.FindProperty("_count");
        _durationProp = serializedObject.FindProperty("_duration");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(_repeatTypeProp);

        var nodeType = (RepeatNode.RepeatType)_repeatTypeProp.enumValueIndex;

        switch (nodeType)
        {
            case RepeatNode.RepeatType.Infinite:
                break;
            case RepeatNode.RepeatType.Count:
                EditorGUILayout.PropertyField(_countProp);
                break;
            case RepeatNode.RepeatType.Duration:
                EditorGUILayout.PropertyField(_durationProp);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
