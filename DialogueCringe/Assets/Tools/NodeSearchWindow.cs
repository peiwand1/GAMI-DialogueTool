using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueGraphView _graphView;
    private EditorWindow _editorWindow;
    private Texture2D _indentationIcon;
    
    public void Init(EditorWindow editorWindow, DialogueGraphView graphView)
    {
        _editorWindow = editorWindow;
        _graphView = graphView;
        _indentationIcon = _graphView._indentationIcon;
    }
    
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeEntry(new GUIContent("Dialogue Node", _indentationIcon))            
            {
                userData = new DialogueNode(), level = 1
            }
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(
            _editorWindow.rootVisualElement.parent, context.screenMousePosition - _editorWindow.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        switch (SearchTreeEntry.userData)
        {
            case DialogueNode dialogueNode:
                _graphView.CreateNode("Dialogue Node", localMousePosition);
                return true;
            default:
                return false;
        }
    }
}
