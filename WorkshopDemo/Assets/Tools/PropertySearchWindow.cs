using System;
using System.Collections.Generic;
using System.Linq;
using Tools.Runtime.Properties;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class PropertySearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueGraphView _graphView;
    private EditorWindow _editorWindow;
    private Texture2D _indentationIcon;
    private Blackboard _blackboard;

    public void Init(EditorWindow editorWindow, DialogueGraphView graphView)
    {
        _editorWindow = editorWindow;
        _graphView = graphView;
        _indentationIcon = new Texture2D(1, 1);
        _indentationIcon.SetPixel(0,0, new Color(0,0,0,0));
        _indentationIcon.Apply();
    }
    
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>()
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeEntry(new GUIContent("String Property", _indentationIcon))
            {
                userData = "String", level = 1
            },
            new SearchTreeEntry(new GUIContent("Boolean Property", _indentationIcon))
            {
                userData = "Boolean", level = 1
            },
            new SearchTreeEntry(new GUIContent("Integer Property", _indentationIcon))
            {
                userData = "Integer", level = 1
            },
            new SearchTreeEntry(new GUIContent("Float Property", _indentationIcon))
            {
                userData = "Float", level = 1
            }
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        switch (SearchTreeEntry.userData)
        {
            case "String":
                _graphView.AddPropertyToBlackboard(new ExposedProperty("New String Property","String"));
                return true;
            case "Boolean":
                _graphView.AddPropertyToBlackboard(new ExposedProperty("New Boolean Property","Boolean"));
                return true;
            case "Integer":
                _graphView.AddPropertyToBlackboard(new ExposedProperty("New Integer Property","Integer"));
                return true;
            case "Float":
                _graphView.AddPropertyToBlackboard(new ExposedProperty("New Float Property","Float"));
                return true;
            default:
                return false;
        }
    }
}