using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CompositeNode : Node
{
	[HideInInspector] public List<Node> children = new List<Node>();
    protected int current = 0;

    public override Node Clone()
    {
        CompositeNode clone = Instantiate(this);
        clone.children = children.ConvertAll(c => c.Clone());
        return clone;
    }

    virtual public void Add(Node node)
    {
        children.Add(node);
    }

    virtual public void Remove(Node node)
    {
        children.Remove(node);
    }
}
