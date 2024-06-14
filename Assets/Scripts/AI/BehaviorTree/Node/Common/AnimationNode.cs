using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationNode : ActionNode
{
    public enum AnimNodeType
    {
        Float,
        Int,
        Bool,
        Trigger,
    }

    [HideInInspector][SerializeField] private AnimNodeType _nodeType = AnimNodeType.Trigger;
    [HideInInspector][SerializeField] private string _floatParam;
    [HideInInspector][SerializeField] private float _floatValue;
    [HideInInspector][SerializeField] private string _intParam;
    [HideInInspector][SerializeField] private int _intValue;
    [HideInInspector][SerializeField] private string _boolParam;
    [HideInInspector][SerializeField] private bool _boolValue;
    [HideInInspector][SerializeField] private string _trigger;

    protected override void OnStart()
    {
        switch (_nodeType)
        {
            case AnimNodeType.Float:
                blackboard.animator.SetFloat(_floatParam, _floatValue);
                break;
            case AnimNodeType.Int:
                blackboard.animator.SetInteger(_intParam, _intValue);
                break;
            case AnimNodeType.Bool:
                blackboard.animator.SetBool(_boolParam, _boolValue);
                break;
            case AnimNodeType.Trigger:
                blackboard.animator.SetTrigger(_trigger);
                break;
        }
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (blackboard.animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            return State.Running;
        else
            return State.Success;
    }
}
