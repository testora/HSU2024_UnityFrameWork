using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogNode : ActionNode
{
    public string message;

    protected override void OnStart()
    {
        Debug.Log($"{message}: OnStart");
    }

    protected override void OnStop()
    {
        Debug.Log($"{message}: OnStop");
    }

    protected override State OnUpdate()
    {
        Debug.Log($"{message}: OnUpdate");
        return State.Success;
    }
}
