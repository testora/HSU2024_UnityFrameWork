using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Unity.VisualScripting;
using TreeEditor;

[CreateAssetMenu()]
public class BehaviorTree : ScriptableObject
{
	public Node rootNode;
	public Node.State treeState = Node.State.Running;
	public Blackboard blackboard = new Blackboard();
	public List<Node> nodes = new List<Node>();

	public void Initialize(MonoBehaviour monoBehaviour)
	{
		blackboard.Initialize(monoBehaviour);
	}

	public Node.State Update()
	{
		if (treeState == Node.State.Running)
		{
			treeState = rootNode.Update();
		}

		return treeState;
	}

#if UNITY_EDITOR
	public Node CreateNode(System.Type type)
	{
		Node node = ScriptableObject.CreateInstance(type) as Node;
		node.name = type.Name;
		node.guid = GUID.Generate().ToString();

		Undo.RecordObject(this, "Behavior Tree (Create Node)");
		nodes.Add(node);

		if (!Application.isPlaying)
		{
			AssetDatabase.AddObjectToAsset(node, this);
		}
		Undo.RegisterCreatedObjectUndo(node, "Behavior Tree (Create Node)");
		
		AssetDatabase.SaveAssets();

		return node;
	}

	public void DeleteNode(Node node)
	{
		Undo.RecordObject(this, "Behavior Tree (Delete Node)");
		nodes.Remove(node);

	//	AssetDatabase.RemoveObjectFromAsset(node);
		Undo.DestroyObjectImmediate(node);
		AssetDatabase.SaveAssets();
	}

	public void AddChild(Node parent, Node child)
	{
		CompositeNode composite = parent as CompositeNode;
		if (composite)
		{
			Undo.RecordObject(composite, "Behavior Tree (Add Child)");
			composite.Add(child);
			EditorUtility.SetDirty(composite);
		}

		DecoratorNode decorator = parent as DecoratorNode;
		if (decorator)
		{
			Undo.RecordObject(decorator, "Behavior Tree (Add Child)");
			decorator.child = child;
			EditorUtility.SetDirty(decorator);
		}

		RootNode root = parent as RootNode;
		if (root)
		{
			Undo.RecordObject(root, "Behavior Tree (Add Child)");
			root.child = child;
			EditorUtility.SetDirty(root);
		}
	}

	public void RemoveChild(Node parent, Node child)
	{

		CompositeNode composite = parent as CompositeNode;
		if (composite)
		{
			Undo.RecordObject(composite, "Behavior Tree (Remove Child)");
			composite.Remove(child);
			EditorUtility.SetDirty(composite);
		}

		DecoratorNode decorator = parent as DecoratorNode;
		if (decorator)
		{
			Undo.RecordObject(decorator, "Behavior Tree (Remove Child)");
			decorator.child = null;
			EditorUtility.SetDirty(decorator);
		}

		RootNode root = parent as RootNode;
		if (root)
		{
			Undo.RecordObject(root, "Behavior Tree (Remove Child)");
			root.child = null;
			EditorUtility.SetDirty(root);
		}
	}

	public List<Node> GetChildren(Node parent)
	{
		List<Node> children = new List<Node>();

		CompositeNode composite = parent as CompositeNode;
		if (composite)
		{
			return composite.children;
		}

		DecoratorNode decorator = parent as DecoratorNode;
		if (decorator && decorator.child != null)
		{
			children.Add(decorator.child);
		}

		RootNode root = parent as RootNode;
		if (root && root.child != null)
		{
			children.Add(root.child);
		}

		return children;
	}
#endif

	public void Traverse(Node node, System.Action<Node> visiter)
	{
		if (node)
		{
			visiter.Invoke(node);
			List<Node> children = GetChildren(node);
			children.ForEach((n) => Traverse(n, visiter));
		}
	}

	public BehaviorTree Clone()
	{
		BehaviorTree tree = Instantiate(this);
		tree.rootNode = rootNode.Clone();
		tree.nodes = new List<Node>();
		Traverse(tree.rootNode, (n) =>
		{
			tree.nodes.Add(n);
		});

		return tree;
	}

	// AIAgent
	public void Bind()
	{
		Traverse(rootNode, (node) =>
		{
			node.blackboard = blackboard;
		});
	}
}
