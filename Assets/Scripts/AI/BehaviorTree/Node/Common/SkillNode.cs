using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RepeatNode;

public class SkillNode : DecoratorNode
{
    [HideInInspector]
    public string selectedSkillName;

    private bool skillAvailable = false;

    protected override void OnStart()
    {
        if (blackboard.skillController.UseSkill(selectedSkillName))
        {
            skillAvailable = true;
        }
        else
        {
            skillAvailable = false;
        }
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (skillAvailable)
        {
            return child.Update();
        }

        return State.Failure;
    }
}
