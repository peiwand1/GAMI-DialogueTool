using UnityEditor.Experimental.GraphView;

public class DialogueNode : Node
{
    public string GUID;
    public string dialogueText;
    public bool Entrypoint = false;
    public string Type = "Dialogue";
}