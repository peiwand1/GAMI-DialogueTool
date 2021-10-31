using System.Collections.Generic;
using Tools.Runtime.Properties;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PropertySearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueGraphView _graphView;
    private Texture2D _indentationIcon;
    private Blackboard _blackboard;

    public void Init(DialogueGraphView graphView)
    {
        _graphView = graphView;
        _indentationIcon = _graphView._indentationIcon;
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
                AddPropertyToBlackboard("New String", "String");
                return true;
            case "Boolean":
                AddPropertyToBlackboard("New Boolean", "Boolean");
                return true;
            case "Integer":
                AddPropertyToBlackboard("New Integer", "Integer");
                return true;
            case "Float":
                AddPropertyToBlackboard("New Float", "Float");
                return true;
            default:
                return false;
        }
    }

    private void AddPropertyToBlackboard(string propertyName, string propertyType)
    {
        _graphView.AddPropertyToBlackboard(new ExposedProperty(propertyName, propertyType));
        _graphView.RefreshDropdown(propertyType);
    }
}