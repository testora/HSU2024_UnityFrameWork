using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(SkillController))]
public class SkillControllerEditor : Editor
{
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        SkillController controller = (SkillController)target;

        reorderableList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("skillContainer.skills"),
                true, true, true, true);

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            //  float spacing = 5f;
            string[] labels = { "Skill Name", "Stock", "CoolTime" };
            float divWidth = EditorStyles.label.CalcSize(new GUIContent("/")).x;
            float spacing = divWidth;
            float labelWidth = labels.Max(s => EditorStyles.label.CalcSize(new GUIContent(s)).x); ;
            float fieldWidth = (rect.width - labelWidth - spacing * 3) * 0.5f;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), labels[0]);
            EditorGUI.PropertyField(
                new Rect(rect.x + labelWidth + spacing, rect.y, rect.width - labelWidth - spacing, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("skillName"), GUIContent.none);

            rect.y += EditorGUIUtility.singleLineHeight + spacing;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), new GUIContent(labels[1], "Current Stock / Max Stock"));
            EditorGUI.PropertyField(
                new Rect(rect.x + labelWidth + spacing, rect.y, fieldWidth - divWidth * 0.5f, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("currentStock"), GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.x + labelWidth + spacing + fieldWidth + spacing - divWidth * 0.5f, rect.y, spacing, EditorGUIUtility.singleLineHeight), "/");
            EditorGUI.PropertyField(
                new Rect(rect.x + labelWidth + spacing * 2 + fieldWidth + spacing + divWidth * 0.5f, rect.y, fieldWidth - divWidth * 0.5f, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("maxStock"), GUIContent.none);

            rect.y += EditorGUIUtility.singleLineHeight + spacing;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), new GUIContent(labels[2], "Current CoolTime / CoolTime"));
            EditorGUI.PropertyField(
                new Rect(rect.x + labelWidth + spacing, rect.y, fieldWidth - divWidth * 0.5f, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("currentCoolTime"), GUIContent.none);
            EditorGUI.LabelField(new Rect(rect.x + labelWidth + spacing + fieldWidth + spacing - divWidth * 0.5f, rect.y, spacing, EditorGUIUtility.singleLineHeight), "/");
            EditorGUI.PropertyField(
                new Rect(rect.x + labelWidth + spacing * 2 + fieldWidth + spacing + divWidth * 0.5f, rect.y, fieldWidth - divWidth * 0.5f, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("coolTime"), GUIContent.none);
        };

        reorderableList.elementHeightCallback = (int index) =>
        {
            // Calculate height for each element
            float spacing = 5f;
            return EditorGUIUtility.singleLineHeight * 3 + spacing * 4;
        };

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Skill Container");
        };

        reorderableList.onAddCallback = (ReorderableList l) =>
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("skillName").stringValue = GetUniqueVariableName(controller.skillContainer);
            element.FindPropertyRelative("maxStock").intValue = 1;
            element.FindPropertyRelative("coolTime").floatValue = 1f;
            element.FindPropertyRelative("currentStock").intValue = 1;
            element.FindPropertyRelative("currentCoolTime").floatValue = 0f;
        };

        reorderableList.onRemoveCallback = (ReorderableList l) =>
        {
            ReorderableList.defaultBehaviours.DoRemoveButton(l);
        };
    }

    private string GetUniqueVariableName(SkillContainer skillContainer)
    {
        int count = 1;
        string baseName = "NewVariable";
        string uniqueName = baseName;

        while (skillContainer.skills.Any(v => v.skillName == uniqueName))
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

        SkillController controller = (SkillController)target;

        for (int i = 0; i < controller.skillContainer.skills.Count; i++)
        {
            var variable = controller.skillContainer.skills[i];
            if (controller.skillContainer.skills.Count(v => v.skillName == variable.skillName) > 1)
            {
                EditorGUILayout.HelpBox($"Variable name '{variable.skillName}' is duplicated. Please use a unique name.", MessageType.Error);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
