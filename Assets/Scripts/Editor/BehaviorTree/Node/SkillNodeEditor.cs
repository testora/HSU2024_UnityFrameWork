using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(SkillNode))]
public class SkillNodeEditor : Editor
{
    private int selectedSkillIndex = 0;
    private string[] skillNames;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SkillNode skillNode = (SkillNode)target;

        SkillController skillController = FindObjectOfType<SkillController>();
        if (skillController != null)
        {
            List<Skill> skills = skillController.skillContainer.skills;

            EditorGUILayout.Space();

            if (skills != null && skills.Count > 0)
            {
                skillNames = new string[skills.Count];
                for (int i = 0; i < skills.Count; i++)
                {
                    skillNames[i] = skills[i].skillName;
                }

                selectedSkillIndex = Mathf.Max(0, System.Array.IndexOf(skillNames, skillNode.selectedSkillName));

                selectedSkillIndex = EditorGUILayout.Popup("Skill", selectedSkillIndex, skillNames);

                skillNode.selectedSkillName = skillNames[selectedSkillIndex];
            }
            else
            {
                EditorGUILayout.LabelField("No skills available in the SkillController.");
            }
        }
        else
        {
            EditorGUILayout.LabelField("SkillController not found in the scene.");
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(skillNode);
        }
    }
}
