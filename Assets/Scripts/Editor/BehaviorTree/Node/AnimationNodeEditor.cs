using Codice.CM.Common;
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(AnimationNode))]
public class AnimationNodeEditor : Editor
{
    SerializedProperty _animNodeTypeProp;
    SerializedProperty _floatParamProp;
    SerializedProperty _floatValueProp;
    SerializedProperty _intParamProp;
    SerializedProperty _intValueProp;
    SerializedProperty _boolParamProp;
    SerializedProperty _boolValueProp;
    SerializedProperty _triggerProp;

    private void OnEnable()
    {
        if (target == null)
            return;

        _animNodeTypeProp = serializedObject.FindProperty("_nodeType");
        _floatParamProp = serializedObject.FindProperty("_floatParam");
        _floatValueProp = serializedObject.FindProperty("_floatValue");
        _intParamProp = serializedObject.FindProperty("_intParam");
        _intValueProp = serializedObject.FindProperty("_intValue");
        _boolParamProp = serializedObject.FindProperty("_boolParam");
        _boolValueProp = serializedObject.FindProperty("_boolValue");
        _triggerProp = serializedObject.FindProperty("_trigger");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(_animNodeTypeProp);

        var nodeType = (AnimationNode.AnimNodeType)_animNodeTypeProp.enumValueIndex;

        switch (nodeType)
        {
            case AnimationNode.AnimNodeType.Float:
                EditorGUILayout.PropertyField(_floatParamProp);
                EditorGUILayout.PropertyField(_floatValueProp);
                break;
            case AnimationNode.AnimNodeType.Int:
                EditorGUILayout.PropertyField(_intParamProp);
                EditorGUILayout.PropertyField(_intValueProp);
                break;
            case AnimationNode.AnimNodeType.Bool:
                EditorGUILayout.PropertyField(_boolParamProp);
                EditorGUILayout.PropertyField(_boolValueProp);
                break;
            case AnimationNode.AnimNodeType.Trigger:
                EditorGUILayout.PropertyField(_triggerProp);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
