using System.Collections.Generic;
using System.Linq;
using Tools.Runtime.Properties;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;
    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(string fileName)
    {
        if (Edges.Count.Equals(0) || Edges.Find(x => x.output.node.title == "START").Equals(null) || Edges.Equals(null))
        {
            EditorUtility.DisplayDialog("Error", "Start Node must be connected to another node before saving!", "OK");
            return;
        }

        var dialogueContainer = Resources.Load<DialogueContainer>(fileName);
        
        if (dialogueContainer != null)
        {
            ClearDialogueContainer(dialogueContainer);
            
            SaveNodes(dialogueContainer);
            SaveExposedProperties(dialogueContainer);
            
            if (!EditorUtility.IsDirty(dialogueContainer))
            {
                EditorUtility.SetDirty(dialogueContainer);
            }            
            AssetDatabase.SaveAssets();
        }
        else
        {
            dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            SaveNodes(dialogueContainer);
            SaveExposedProperties(dialogueContainer);
        
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
    
            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
            EditorUtility.SetDirty(dialogueContainer);
            AssetDatabase.SaveAssets(); 
        }
    }
    
    private void ClearDialogueContainer(DialogueContainer dialogueContainer)
    {
        dialogueContainer.NodeLinks.Clear();
        dialogueContainer.DialogueNodeData.Clear();
        dialogueContainer.ExposedBooleanProperties.Clear();
        dialogueContainer.ExposedFloatProperties.Clear();
        dialogueContainer.ExposedIntegerProperties.Clear();
        dialogueContainer.ExposedStringProperties.Clear();
    }

    private void SaveNodes(DialogueContainer dialogueContainer)
    {
        if (!Edges.Any()) return;
        
        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

        for(int i =0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as DialogueNode;
            var inputNode = connectedPorts[i].input.node as DialogueNode;

            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = connectedPorts[i].output.portName,
                ConditionBoolean = (string)connectedPorts[i].output.userData,
                TargetNodeGuid = inputNode.GUID
            });
        }

        foreach(var dialogueNode in Nodes.Where(node => !node.Entrypoint))
        {
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
            {
                NodeGUID = dialogueNode.GUID,
                DialogueText = dialogueNode.dialogueText,
                Position = dialogueNode.GetPosition().position
            });
        }
    }

    private void SaveExposedProperties(DialogueContainer dialogueContainer)
    {
        foreach (var exposedProperty in _targetGraphView.ExposedProperties)
        {
            switch (exposedProperty.PropertyType)
            {
                case "String":
                    dialogueContainer.ExposedStringProperties.Add(new ExposedStringPropertyData
                    { 
                        PropertyName = exposedProperty.PropertyName,
                        PropertyValue = exposedProperty.PropertyValue,
                        PropertyType = exposedProperty.PropertyType
                    });
                    break;
                case "Boolean":
                    dialogueContainer.ExposedBooleanProperties.Add(new ExposedBooleanPropertyData
                    { 
                        PropertyName = exposedProperty.PropertyName,
                        PropertyValue = exposedProperty.PropertyValue,
                        PropertyType = exposedProperty.PropertyType
                    });
                    break;
                case "Integer":
                    dialogueContainer.ExposedIntegerProperties.Add(new ExposedIntegerPropertyData
                    { 
                        PropertyName = exposedProperty.PropertyName,
                        PropertyValue = exposedProperty.PropertyValue,
                        PropertyType = exposedProperty.PropertyType
                    });
                    break;
                case "Float":
                    dialogueContainer.ExposedFloatProperties.Add(new ExposedFloatPropertyData
                    { 
                        PropertyName = exposedProperty.PropertyName,
                        PropertyValue = exposedProperty.PropertyValue,
                        PropertyType = exposedProperty.PropertyType
                    });
                    break;
            }
        }
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<DialogueContainer>(fileName);
        if(_containerCache == null)
        {
            EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exists!", "OK");
            return;
        }

        ClearGraph();
        LoadExposedProperties();
        CreateNodes();
        ConnectNodes();
    }

    private void LoadExposedProperties()
    {
        foreach (var exposedPropertyData in _containerCache.ExposedStringProperties)
        {
            ExposedProperty exposedProperty = new ExposedProperty(exposedPropertyData.PropertyName, exposedPropertyData.PropertyType);
            exposedProperty.PropertyValue = exposedPropertyData.PropertyValue;
            _targetGraphView.AddPropertyToBlackboard(exposedProperty);
        }
        foreach (var exposedPropertyData in _containerCache.ExposedBooleanProperties)
        {
            ExposedProperty exposedProperty = new ExposedProperty(exposedPropertyData.PropertyName, exposedPropertyData.PropertyType);
            exposedProperty.PropertyValue = exposedPropertyData.PropertyValue;
            _targetGraphView.AddPropertyToBlackboard(exposedProperty);
        }
        foreach (var exposedPropertyData in _containerCache.ExposedIntegerProperties)
        {
            ExposedProperty exposedProperty = new ExposedProperty(exposedPropertyData.PropertyName, exposedPropertyData.PropertyType);
            exposedProperty.PropertyValue = exposedPropertyData.PropertyValue;
            _targetGraphView.AddPropertyToBlackboard(exposedProperty);
        }
        foreach (var exposedPropertyData in _containerCache.ExposedFloatProperties)
        {
            ExposedProperty exposedProperty = new ExposedProperty(exposedPropertyData.PropertyName, exposedPropertyData.PropertyType);
            exposedProperty.PropertyValue = exposedPropertyData.PropertyValue;
            _targetGraphView.AddPropertyToBlackboard(exposedProperty);
        }
    }

    private void ConnectNodes()
    {
        for (int i = 0; i< Nodes.Count; i++)
        {
            var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].GUID).ToList();
            for(var j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                targetNode.SetPosition(new Rect
                (
                    _containerCache.DialogueNodeData.First(x => x.NodeGUID == targetNodeGuid).Position,
                    _targetGraphView.defaultNodeSize
                ));
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };

        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }

    private void CreateNodes()
    {
        foreach(var nodeData in _containerCache.DialogueNodeData)
        {
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText, Vector2.zero);
            tempNode.GUID = nodeData.NodeGUID;
            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.NodeGUID).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName, x.ConditionBoolean));
        }
    }

    private void ClearGraph()
    {
        Nodes.Find(x => x.Entrypoint).GUID = _containerCache.NodeLinks[0].BaseNodeGuid;
        
        foreach (var perNode in Nodes)
        {
            if (perNode.Entrypoint) continue;
            Edges.Where(x => x.input.node == perNode).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));
            _targetGraphView.RemoveElement(perNode);
        }
        
        _targetGraphView.ClearBlackboardAndExposedProperties();
    }
}
