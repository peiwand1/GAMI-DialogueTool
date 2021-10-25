using System;
using System.Collections;
using System.Collections.Generic;
using Tools.Runtime;
using Tools.Runtime.Properties;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();

    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    
    public List<ExposedPropertyData> ExposedProperties = new List<ExposedPropertyData>();
}
