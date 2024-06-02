using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillContainer
{
    public List<Skill> skills = new List<Skill>();
    private Dictionary<string, Skill> _skillDictionary = new Dictionary<string, Skill>();

    public void Update()
    {
        foreach (Skill skill in skills)
        {
            if (skill.currentStock < skill.maxStock)
            {
                skill.currentCoolTime += Time.deltaTime;
                if (skill.currentCoolTime > skill.coolTime)
                {
                    skill.currentStock++;
                    if (skill.currentStock == skill.maxStock)
                        skill.currentCoolTime = 0f;
                    else
                        skill.currentCoolTime -= skill.coolTime;
                }
            }
        }
    }

    public void Initialize()
    {
        _skillDictionary = new Dictionary<string, Skill>();
        foreach (var skill in skills)
        {
            if (!_skillDictionary.ContainsKey(skill.skillName))
            {
                _skillDictionary.Add(skill.skillName, skill);
            }
        }
    }
}

[System.Serializable]
public class Skill
{
    public string skillName;
    public int maxStock;
    public float coolTime;

    public int currentStock;
    public float currentCoolTime;

    public Skill()
    {
        maxStock = 1;
        coolTime = 1f;
        currentStock = maxStock;
        currentCoolTime = 0f;
    }
}
