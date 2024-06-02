using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement
{
	public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

	Editor _editor;

	public InspectorView()
	{

	}

	internal void UpdateSelection(NodeView nodeView)
	{
		Clear();

		UnityEngine.Object.DestroyImmediate(_editor);

		_editor = Editor.CreateEditor(nodeView.node);
		IMGUIContainer container = new IMGUIContainer(() =>
		{
			if (_editor.target)
			{
				_editor.OnInspectorGUI();
			}
		});
		Add(container);
	}
}
