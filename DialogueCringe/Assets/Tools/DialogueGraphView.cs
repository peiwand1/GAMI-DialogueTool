using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using Tools.Runtime.Properties;
using UnityEditor;
using UnityEditor.UIElements;
using Button = UnityEngine.UIElements.Button;
using Label = UnityEngine.UIElements.Label;


public class DialogueGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    public Blackboard Blackboard;
    public List<ExposedProperty> ExposedProperties = new List<ExposedProperty>();
    private NodeSearchWindow searchWindow;
    private PropertySearchWindow propertySearchWindow;
    public DialogueGraph EditorWindow;
    public Texture2D _indentationIcon;
    public string oldConditionName;
    public string newConditionName;

    public DialogueGraphView(DialogueGraph editorWindow)
    {
        _indentationIcon = new Texture2D(1, 1);
        _indentationIcon.SetPixel(0,0, new Color(0,0,0,0));
        _indentationIcon.Apply();
        
        EditorWindow = editorWindow;
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
        AddNodeSearchWindow(editorWindow);
    }

    private void AddNodeSearchWindow(EditorWindow editorWindow)
    {
        searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        searchWindow.Init(editorWindow, this);
        nodeCreationRequest = context =>
        {
            if (context.screenMousePosition.x == 0 && context.screenMousePosition.y == 0)
            {
                context.screenMousePosition.x = editorWindow.position.x + editorWindow.position.width/2;
                context.screenMousePosition.y = (editorWindow.position.y + editorWindow.position.height/2) - 120;
            }
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        };
    }

    public void OpenNodeSearchWindow()
    {
        nodeCreationRequest.Invoke(default);
    }
    
    public void AddPropertySearchWindow(EditorWindow editorWindow)
    {
        propertySearchWindow = ScriptableObject.CreateInstance<PropertySearchWindow>();
        propertySearchWindow.Init(this);
        Blackboard.addItemRequested = context =>
            SearchWindow.Open(new SearchWindowContext(new Vector2(editorWindow.position.x + 125,editorWindow.position.y + 60)), propertySearchWindow);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) => 
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            dialogueText = "ENTRYPOINT",
            Entrypoint = true
        };

        var generatedNode = GeneratePort(node, Direction.Output);
        generatedNode.portName = "Next";
        node.outputContainer.Add(generatedNode);

        node.capabilities &= ~Capabilities.Deletable;
        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }

    public void CreateNode(string nodeName, Vector2 mousePosition)
    {
        AddElement(CreateDialogueNode(nodeName, mousePosition));
    }

    public DialogueNode CreateDialogueNode(string nodeName, Vector2 mousePosition)
    {
        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            dialogueText = nodeName,
            GUID = Guid.NewGuid().ToString(),
        };
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        var button = new Button(()=> { AddChoicePort(dialogueNode); });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.dialogueText = evt.newValue;
            dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.title);
        dialogueNode.mainContainer.Add(textField);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(mousePosition, defaultNodeSize));

        return dialogueNode;
    }

    public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "", string overrideConditionBoolean = "No condition")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {outputPortCount + 1}" : overriddenPortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };

        var variableDropdown = GenerateDropdown(overrideConditionBoolean);
        
        generatedPort.contentContainer.Add(variableDropdown);
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add( new Label(" "));
        generatedPort.contentContainer.Add(textField);

        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);
        variableDropdown.RegisterValueChangedCallback(evt => generatedPort.userData = evt.newValue);

        generatedPort.portName = choicePortName;
        generatedPort.userData = variableDropdown.value;
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();

    }

    private DropdownField GenerateDropdown(string overrideConditionBoolean)
    {
        var variables = ExposedProperties.Where(x => x.PropertyType == "Boolean").ToList();
        var variableDropdown = new DropdownField();
        variableDropdown.choices.Add("No condition");
        variableDropdown.choices.AddRange(variables.Select(x => x.PropertyName).ToList());
        if (variables.Select(x => x.PropertyName).Contains(overrideConditionBoolean))
        {
            variableDropdown.value = overrideConditionBoolean;
        }
        else if (overrideConditionBoolean.Equals(oldConditionName))
        {
            variableDropdown.value = newConditionName;
        }
        else
        {
            variableDropdown.value = "No condition";
        }
        variableDropdown.style.minWidth = new StyleLength(100);
        variableDropdown.style.maxWidth = new StyleLength(100);
        return variableDropdown;
    }

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }

    public void ClearBlackboardAndExposedProperties()
    {
        ExposedProperties.Clear();
        Blackboard.Clear();
    }

    public void AddPropertyToBlackboard(ExposedProperty exposedProperty)
    {
        var localPropertyName = exposedProperty.PropertyName;
        var localPropertyValue = exposedProperty.PropertyValue;
        var localPropertyType = exposedProperty.PropertyType;

        while (ExposedProperties.Any(x => x.PropertyName.Equals(localPropertyName)))
        {
            localPropertyName = $"{localPropertyName}(1)";
        }

        ExposedProperty property = new ExposedProperty(localPropertyName, localPropertyType);
        property.PropertyName = localPropertyName;
        ExposedProperties.Add(property);

        var container = new VisualElement();
        dynamic propertyValueField = new TextField();
        var blackboardField = new BlackboardField { text = property.PropertyName };

        switch (localPropertyType)
        {
            case "String":
                if (localPropertyValue == null)
                {
                    localPropertyValue = "New String";
                }

                blackboardField.typeText = "string";
                propertyValueField = new TextField("Value:")
                {
                    value = localPropertyValue
                };
                property.PropertyValue = localPropertyValue;

                ((TextField)propertyValueField).RegisterValueChangedCallback(evt =>
                {
                    var changingPropertyIndex =
                        ExposedProperties.FindIndex(x => x.PropertyName.Equals(property.PropertyName));
                    ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
                });

                break;
            case "Boolean":
                if (localPropertyValue == null)
                {
                    localPropertyValue = false;
                }

                blackboardField.typeText = "boolean";
                propertyValueField = new Toggle("Value:")
                {
                    value = localPropertyValue
                };
                property.PropertyValue = localPropertyValue;

                ((Toggle)propertyValueField).RegisterValueChangedCallback(evt =>
                {
                    var changingPropertyIndex =
                        ExposedProperties.FindIndex(x => x.PropertyName.Equals(property.PropertyName));
                    ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
                });

                break;
            case "Integer":
                if (localPropertyValue == null)
                {
                    localPropertyValue = 0;
                }

                blackboardField.typeText = "integer";
                propertyValueField = new IntegerField("Value:")
                {
                    value = localPropertyValue

                };
                property.PropertyValue = localPropertyValue;

                ((IntegerField)propertyValueField).RegisterValueChangedCallback(evt =>
                {
                    var changingPropertyIndex =
                        ExposedProperties.FindIndex(x => x.PropertyName.Equals(property.PropertyName));
                    ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
                });

                break;

            case "Float":
                if (localPropertyValue == null)
                {
                    localPropertyValue = 0.0f;
                }

                blackboardField.typeText = "float";
                propertyValueField = new FloatField("Value:")
                {
                    value = localPropertyValue

                };
                property.PropertyValue = localPropertyValue;

                ((FloatField)propertyValueField).RegisterValueChangedCallback(evt =>
                {            
                    var changingPropertyIndex = ExposedProperties.FindIndex(x => x.PropertyName.Equals(property.PropertyName));
                    ExposedProperties[changingPropertyIndex].PropertyValue = evt.newValue;
                });

                break;
        }

        container.Add(blackboardField);
        var blackboardValueRow = new BlackboardRow(blackboardField, propertyValueField);
        container.Add(blackboardValueRow);

        container.AddManipulator(new ContextualMenuManipulator((evt) =>
        {
            var propertyIndex = ExposedProperties.FindIndex(x => x.PropertyName.Equals(property.PropertyName));
            evt.menu.AppendAction("Delete", (a) => RemovePropertyFromBlackboard(container, propertyIndex, property.PropertyType), DropdownMenuAction.AlwaysEnabled);
        }));
        Blackboard.Add(container);
    }

    private void RemovePropertyFromBlackboard(VisualElement property, int placeInList, string propertyType)
    {
        Blackboard.Remove(property);
        ExposedProperties.RemoveAt(placeInList);

        RefreshDropdown(propertyType);
    }

    public void RefreshDropdown(string propertyType)
    {
        if (propertyType.Equals("Boolean"))
        {
            EditorWindow.RequestDataOperation(true);
            EditorWindow.RequestDataOperation(false);
        }
    }
}