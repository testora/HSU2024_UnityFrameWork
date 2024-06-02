// ControllerEditor.cs
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(DemoController))]
public class DemoControllerEditor : Editor
{
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        DemoController controller = (DemoController)target;

        reorderableList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("myVar.customVariables"),
                true, true, true, true);

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("variableName"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + 105, rect.y, 100, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("type"), GUIContent.none);

            var type = (VariableType)element.FindPropertyRelative("type").enumValueIndex;
            switch (type)
            {
                case VariableType.Int:
                    EditorGUI.PropertyField(
                        new Rect(rect.x + 210, rect.y, rect.width - 210, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("intValue"), GUIContent.none);
                    break;
                case VariableType.Float:
                    EditorGUI.PropertyField(
                        new Rect(rect.x + 210, rect.y, rect.width - 210, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("floatValue"), GUIContent.none);
                    break;
                case VariableType.String:
                    EditorGUI.PropertyField(
                        new Rect(rect.x + 210, rect.y, rect.width - 210, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("stringValue"), GUIContent.none);
                    break;
                case VariableType.Bool:
                    EditorGUI.PropertyField(
                        new Rect(rect.x + 210, rect.y, rect.width - 210, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("boolValue"), GUIContent.none);
                    break;
            }
        };

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Custom Variables");
        };

        reorderableList.onAddCallback = (ReorderableList l) =>
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("variableName").stringValue = GetUniqueVariableName(controller.myVar);
            element.FindPropertyRelative("type").enumValueIndex = 0;
            element.FindPropertyRelative("intValue").intValue = 0;
            element.FindPropertyRelative("floatValue").floatValue = 0f;
            element.FindPropertyRelative("stringValue").stringValue = "";
            element.FindPropertyRelative("boolValue").boolValue = false;
        };

        reorderableList.onRemoveCallback = (ReorderableList l) =>
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(l);
        };
    }

    private string GetUniqueVariableName(DemoVariables variables)
    {
        int count = 1;
        string baseName = "NewVariable";
        string uniqueName = baseName;

        while (variables.customVariables.Any(v => v.variableName == uniqueName))
        {
            uniqueName = baseName + count;
            count++;
        }

        return uniqueName;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        serializedObject.Update();

        reorderableList.DoLayoutList();

        DemoController controller = (DemoController)target;

        for (int i = 0; i < controller.myVar.customVariables.Count; i++)
        {
            var variable = controller.myVar.customVariables[i];
            if (controller.myVar.customVariables.Count(v => v.variableName == variable.variableName) > 1)
            {
                EditorGUILayout.HelpBox($"Variable name '{variable.variableName}' is duplicated. Please use a unique name.", MessageType.Error);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
