using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ParallelNode : CompositeNode
{
    public enum Policy
    {
        RequireAll,
        RequireAny,
        RequireOne,
    }

    [HideInInspector][SerializeField] private Policy _successPolicy = Policy.RequireAll;
    [HideInInspector][SerializeField] private Policy _failurePolicy = Policy.RequireAll;
    [HideInInspector][SerializeField] private Node _requiredNode;
    private Dictionary<Node, State> states = new Dictionary<Node, State>();

    protected override void OnStart()
    {
        states = states.ToDictionary(kvp => kvp.Key, kvp => State.Running);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        bool allSuccess = true;
        bool anySuccess = false;
        bool allFailure = true;
        bool anyFailure = false;

        for (int i = 0; i < children.Count; i++)
        {
            if (states[children[i]] == State.Running)
                states[children[i]] = children[i].Update();

            switch (states[children[i]])
            {
                case State.Running:
                    allSuccess = false;
                    allFailure = false;
                    break;
                case State.Success:
                    allFailure = false;
                    anySuccess = true;
                    break;
                case State.Failure:
                    allSuccess = false;
                    anyFailure = true;
                    break;
            }
        }

        switch (_successPolicy)
        {
            case Policy.RequireAll:
                if (allSuccess)
                    return State.Success;
                break;
            case Policy.RequireAny:
                if (anySuccess)
                    return State.Success;
                break;
            case Policy.RequireOne:
                if (states[_requiredNode] == State.Success)
                    return State.Success;
                break;
        }

        switch (_failurePolicy)
        {
            case Policy.RequireAll:
                if (allFailure)
                    return State.Failure;
                break;
            case Policy.RequireAny:
                if (anyFailure)
                    return State.Failure;
                break;
            case Policy.RequireOne:
                if (states[_requiredNode] == State.Failure)
                    return State.Failure;
                break;
        }

        return State.Running;
    }

    public override Node Clone()
    {
        ParallelNode clone = (ParallelNode)base.Clone();
        clone._requiredNode = clone.children[children.IndexOf(_requiredNode)];
        clone.states = new Dictionary<Node, State>();
        for (int i = 0; i < clone.children.Count; ++i)
        {
            clone.states.Add(clone.children[i], states[children[i]]);
        }

        return clone;
    }

    public override void Add(Node node)
    {
        base.Add(node);
        states.Add(node, State.Running);
    }

    public override void Remove(Node node)
    {
        base.Remove(node);
        states.Remove(node);
    }
}
