using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillController : MonoBehaviour
{
    [HideInInspector]
    public SkillContainer skillContainer = new SkillContainer();

    void Awake()
    {
        skillContainer.Initialize();
    }

    void Update()
    {
        skillContainer.Update();
    }

    public bool UseSkill(string skillName)
    {
        return skillContainer.UseSkill(skillName);
    }
}
