using Codice.CM.Common;
using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ParallelNode))]
public class ParallelNodeEditor : Editor
{
    SerializedProperty _successPolicyProp;
    SerializedProperty _failurePolicyProp;
    SerializedProperty _requiredNodeProp;

    private void OnEnable()
    {
        if (target == null)
            return;

        _successPolicyProp = serializedObject.FindProperty("_successPolicy");
        _failurePolicyProp = serializedObject.FindProperty("_failurePolicy");
        _requiredNodeProp = serializedObject.FindProperty("_requiredNode");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(_successPolicyProp);
        EditorGUILayout.PropertyField(_failurePolicyProp);

        ParallelNode.Policy successPolicy = (ParallelNode.Policy)_successPolicyProp.enumValueIndex;
        ParallelNode.Policy failurePolicy = (ParallelNode.Policy)_failurePolicyProp.enumValueIndex;
        if (successPolicy == ParallelNode.Policy.RequireOne || failurePolicy == ParallelNode.Policy.RequireOne)
        {
            ParallelNode node = (ParallelNode)target;

            if (node.children != null && node.children.Count > 0)
            {
                string[] childNames = node.children.Select(child => child.name).ToArray();
                int currentIndex = Array.IndexOf(childNames, _requiredNodeProp.objectReferenceValue ? _requiredNodeProp.objectReferenceValue.name : null);

                int selectedIndex = EditorGUILayout.Popup("Required Node", currentIndex, childNames);
                if (selectedIndex >= 0 && selectedIndex < node.children.Count)
                {
                    _requiredNodeProp.objectReferenceValue = node.children[selectedIndex];
                }
                else
                {
                    _requiredNodeProp.objectReferenceValue = null;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No child nodes available.", MessageType.Warning);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
