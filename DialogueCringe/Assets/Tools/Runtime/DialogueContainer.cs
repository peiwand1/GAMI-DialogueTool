using System.Collections;
using System.Collections.Generic;
using Tools.Runtime;
using Tools.Runtime.Properties;
using UnityEngine;

public class DialogueContainer : ScriptableObject
{
    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
    public List<ExposedStringProperty> ExposedProperties = new List<ExposedStringProperty>();
}
