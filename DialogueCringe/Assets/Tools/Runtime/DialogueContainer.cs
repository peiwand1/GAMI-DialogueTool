using System;
using System.Collections.Generic;
using Tools.Runtime.Properties;
using UnityEngine;

[Serializable]
public class DialogueContainer : ScriptableObject
{
    public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();

    public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
    
    public List<ExposedStringPropertyData> ExposedStringProperties = new List<ExposedStringPropertyData>();
    
    public List<ExposedBooleanPropertyData> ExposedBooleanProperties = new List<ExposedBooleanPropertyData>();
    
    public List<ExposedIntegerPropertyData> ExposedIntegerProperties = new List<ExposedIntegerPropertyData>();
    
    public List<ExposedFloatPropertyData> ExposedFloatProperties = new List<ExposedFloatPropertyData>();
}
