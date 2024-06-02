using Codice.CM.Common.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BehaviorTreeView : GraphView
{
	public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }

	public Action<NodeView> onNodeSelected;
	BehaviorTree _tree;

	public BehaviorTreeView()
	{
		Insert(0, new GridBackground());

		this.AddManipulator(new ContentZoomer());
		this.AddManipulator(new ContentDragger());
		this.AddManipulator(new SelectionDragger());
		this.AddManipulator(new RectangleSelector());

		var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Data/UIBuilder/BehaviorTreeEditor.uss");
		styleSheets.Add(styleSheet);

		Undo.undoRedoPerformed += OnUndoRedo;
	}

    private void OnUndoRedo()
    {
		PopulateView(_tree);
		AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(Node node)
	{
		return GetNodeByGuid(node.guid) as NodeView;
	}

	internal void PopulateView(BehaviorTree tree)
	{
		if (tree == null)
			return;

		_tree = tree;

		graphViewChanged -= OnGraphViewChanged;
		DeleteElements(graphElements);
		graphViewChanged += OnGraphViewChanged;

		if (_tree.rootNode == null)
		{
			tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
			EditorUtility.SetDirty(tree);
			AssetDatabase.SaveAssets();
		}

		// Create NodeView
		tree.nodes.ForEach(n => CreateNodeView(n));

        // Create Edge
        tree.nodes.ForEach(n =>
		{
			List<Node> children = tree.GetChildren(n);
			children.ForEach(c =>
			{
				NodeView parentView = FindNodeView(n);
				NodeView childView = FindNodeView(c);

				Edge edge = parentView.output.ConnectTo(childView.input);
				AddElement(edge);
			});
		});
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
		return ports.ToList().Where(endport =>
		endport.direction != startPort.direction &&
		endport.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
	{
		if (graphViewChange.elementsToRemove != null)
		{
			graphViewChange.elementsToRemove.ForEach(elem =>
			{
				NodeView nodeView = elem as NodeView;
				if (nodeView != null)
				{
					_tree.DeleteNode(nodeView.node);
				}

				Edge edge = elem as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    _tree.RemoveChild(parentView.node, childView.node);
                }
            });
		}

		if (graphViewChange.edgesToCreate != null)
		{
			graphViewChange.edgesToCreate.ForEach(edge =>
			{
				NodeView parentView = edge.output.node as NodeView;
				NodeView childView = edge.input.node as NodeView;
				_tree.AddChild(parentView.node, childView.node);
			});
		}

		if (graphViewChange.movedElements != null)
		{
			nodes.ForEach((n) =>
			{
				NodeView view = n as NodeView;
				view.SortChildren();
			});
		}

		return graphViewChange;
	}

	public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
	{
		string basePath = "Assets/Scripts/AI/BehaviorTree/Node/";

        TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<Node>();

        foreach (Type type in types)
        {
            if (type.IsAbstract) continue;

            string[] guids = AssetDatabase.FindAssets($"{type.Name} t:script");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]).Replace(basePath, "");
                evt.menu.AppendAction($"{Path.GetDirectoryName(path)}/{type.BaseType.Name}/{type.Name}", _ => CreateNode(type));
            }
        }

        base.BuildContextualMenu(evt);
    }

	void CreateNode(System.Type type)
	{
		Node node = _tree.CreateNode(type);
		CreateNodeView(node);
	}

	void CreateNodeView(Node node)
	{
		NodeView nodeView = new NodeView(node);
		nodeView.onNodeSelected = onNodeSelected;
		AddElement(nodeView);
	}

	public void UpdateNodeStates()
	{
		nodes.ForEach(n =>
		{
			NodeView view = n as NodeView;
			view.UpdateState();
		});
	}
}
