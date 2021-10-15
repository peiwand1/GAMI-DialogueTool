using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueGraphView graphView;
    private EditorWindow editorWindow;
    private Texture2D indentationIcon;
    
    public void Init(EditorWindow editorWindow, DialogueGraphView graphView)
    {
        this.editorWindow = editorWindow;
        this.graphView = graphView;
        indentationIcon = new Texture2D(1, 1);
        indentationIcon.SetPixel(0,0, new Color(0,0,0,0));
        indentationIcon.Apply();
    }
    
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
            new SearchTreeEntry(new GUIContent("Dialogue Node", indentationIcon))
            {
                userData = new DialogueNode(), level = 2
            }
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(
            editorWindow.rootVisualElement.parent, context.screenMousePosition - editorWindow.position.position);
        var localMousePosition = graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        switch (SearchTreeEntry.userData)
        {
            case DialogueNode dialogueNode:
                graphView.CreateNode("Dialogue Node", localMousePosition);
                return true;
            default:
                return false;
        }
    }
}
