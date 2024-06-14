using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditorInternal;
using System.Linq;

public class BehaviorTreeEditor : EditorWindow
{
    BehaviorTreeView _treeView;
    InspectorView _inspectorView;
    IMGUIContainer _blackboardView;

    SerializedObject _treeObject;
    SerializedProperty _blackboardProperty;
    ReorderableList _variableList;

    [MenuItem("BehaviorTree/Editor")]
    public static void OpenWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        if (Selection.activeObject is BehaviorTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Data/UIBuilder/BehaviorTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Data/UIBuilder/BehaviorTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        _treeView = root.Q<BehaviorTreeView>();
        _inspectorView = root.Q<InspectorView>();
        _blackboardView = root.Q<IMGUIContainer>();
        _blackboardView.onGUIHandler = DrawBlackboard;

        _treeView.onNodeSelected = OnNodeSelectionChanged;
        _treeView.focusable = true;

        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {

    }

    private void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        switch (change)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }

    private void OnSelectionChange()
    {
        BehaviorTree tree = Selection.activeObject as BehaviorTree;
        if (!tree)
        {
            if (Selection.activeGameObject)
            {
                BehaviorTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();
                if (runner)
                {
                    tree = runner.tree;
                }
            }
        }

        if (Application.isPlaying)
        {
            if (tree)
            {
                _treeView.PopulateView(tree);
            }
        }
        else
        {
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                _treeView.PopulateView(tree);
            }
        }

        if (tree != null)
        {
            _treeObject = new SerializedObject(tree);
            _blackboardProperty = _treeObject.FindProperty("blackboard");

            _variableList = new ReorderableList(_treeObject,
                    _blackboardProperty.FindPropertyRelative("dynamicFields"),
                    true, true, true, true);

            _variableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = _variableList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("variableName"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 105, rect.y, 100, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("type"), GUIContent.none);

                var type = (VariableType)element.FindPropertyRelative("type").enumValueIndex;
                switch (type)
                {
                    case VariableType.Int:
                        EditorGUI.PropertyField(
                            new Rect(rect.x + 210, rect.y, rect.width - 210, EditorGUIUtility.singleLineHeight),
                            element.FindPropertyRelative("intValue"), GUIContent.none);
                        break;
                    case VariableType.Float:
                        EditorGUI.PropertyField(
                            new Rect(rect.x + 210, rect.y, rect.width - 210, EditorGUIUtility.singleLineHeight),
                            element.FindPropertyRelative("floatValue"), GUIContent.none);
                        break;
                    case VariableType.String:
                        EditorGUI.PropertyField(
                            new Rect(rect.x + 210, rect.y, rect.width - 210, EditorGUIUtility.singleLineHeight),
                            element.FindPropertyRelative("stringValue"), GUIContent.none);
                        break;
                    case VariableType.Bool:
                        EditorGUI.PropertyField(
                            new Rect(rect.x + 210, rect.y, rect.width - 210, EditorGUIUtility.singleLineHeight),
                            element.FindPropertyRelative("boolValue"), GUIContent.none);
                        break;
                }
            };

            _variableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Dynamic Fields");
            };

            _variableList.onAddCallback = (ReorderableList l) =>
            {
                var index = l.serializedProperty.arraySize;
                l.serializedProperty.arraySize++;
                l.index = index;
                var element = l.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("variableName").stringValue = GetUniqueVariableName(tree.blackboard);
                element.FindPropertyRelative("type").enumValueIndex = 0;
                element.FindPropertyRelative("intValue").intValue = 0;
                element.FindPropertyRelative("floatValue").floatValue = 0f;
                element.FindPropertyRelative("stringValue").stringValue = "";
                element.FindPropertyRelative("boolValue").boolValue = false;
            };

            _variableList.onRemoveCallback = (ReorderableList l) =>
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            };
        }
    }

    private string GetUniqueVariableName(Blackboard blackboard)
    {
        int count = 1;
        string baseName = "NewVariable";
        string uniqueName = baseName;

        while (blackboard.dynamicFields.Any(v => v.variableName == uniqueName))
        {
            uniqueName = baseName + count;
            count++;
        }

        return uniqueName;
    }

    private void OnNodeSelectionChanged(NodeView nodeView)
    {
        _inspectorView.UpdateSelection(nodeView);
    }

    private void DrawBlackboard()
    {
        if (_treeObject == null || _blackboardProperty == null || _variableList == null)
            return;

        _treeObject.Update();
    //  EditorGUILayout.PropertyField(_blackboardProperty.FindPropertyRelative("animator"));
    //  EditorGUILayout.PropertyField(_blackboardProperty.FindPropertyRelative("skillController"));
        _variableList.DoLayoutList();
        _treeObject.ApplyModifiedProperties();
    }

    private void OnInspectorUpdate()
    {
        _treeView?.UpdateNodeStates();
    }
}
