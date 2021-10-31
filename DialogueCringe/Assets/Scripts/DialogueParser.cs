using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueParser : MonoBehaviour
{

    [SerializeField] private Button choicePrefab;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private ChoiceHandler choiceHandler;
    
    private DialogueContainer dialogue;
    private TextMeshProUGUI dialogueText;
    private Transform buttonContainer;

    private void Start() {
        dialogueText = dialogueBox.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        buttonContainer = dialogueBox.transform.Find("ButtonHandler").GetComponent<Transform>();
    }

    public void EnableDialogue() {
        dialogueBox.gameObject.SetActive(true);
    }

    public void DisableDialogue() {
        dialogueBox.gameObject.SetActive(false);
    }

    public void StartDialogue(DialogueContainer dialogue) {
        this.dialogue = dialogue;
        ProceedToNarrative(dialogue.NodeLinks.First().TargetNodeGuid);
    }  

    private void ProceedToNarrative(string narrativeDataGUID)
    {
        RemoveButtons();
        var text = dialogue.DialogueNodeData.Find(x => x.NodeGUID == narrativeDataGUID).DialogueText;
        dialogueText.text = ProcessProperties(text);
        AddChoiceButtons(narrativeDataGUID);
    }

    private void RemoveButtons() {
       var buttons = buttonContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            Destroy(buttons[i].gameObject);
        } 
    }

    private void AddChoiceButtons(string narrativeDataGUID) {
        var choices = dialogue.NodeLinks.Where(x => x.BaseNodeGuid == narrativeDataGUID);
        foreach (var choice in choices)
        {
            var button = Instantiate(choicePrefab, buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = ProcessProperties(choice.PortName);
            //button.GetComponentInChildren<TextMeshProUGUI>().text = (choice.PortName);

            button.onClick.AddListener(() => HandleChoice(choice));

            // if bool is false, disable button
            var property = dialogue.ExposedBooleanProperties.Find(x => x.PropertyName == choice.ConditionBoolean);
            if (property != null && !property.PropertyValue)
            {
                button.interactable = false;
            }
        }
    }
    
    private void HandleChoice(NodeLinkData choice) {
        choiceHandler.HandleChoice(choice);
        ProceedToNarrative(choice.TargetNodeGuid);
    }

    private string ProcessProperties(string text)
    {
        string newText = text;
        if (text.Contains("{"))
        {
            foreach (var exposedProperty in dialogue.ExposedStringProperties)
            {
                text = text.Replace($"{exposedProperty.PropertyName}", exposedProperty.PropertyValue);
                newText = text.Replace("{", "").Replace("}", "");
            }

            foreach (var exposedProperty in dialogue.ExposedBooleanProperties)
            {
                text = text.Replace($"{exposedProperty.PropertyName}", exposedProperty.PropertyValue.ToString());
                newText = text.Replace("{", "").Replace("}", "");
            }

            foreach (var exposedProperty in dialogue.ExposedIntegerProperties)
            {
                text = text.Replace($"{exposedProperty.PropertyName}", exposedProperty.PropertyValue.ToString());
                newText = text.Replace("{", "").Replace("}", "");
            }

            foreach (var exposedProperty in dialogue.ExposedFloatProperties)
            {
                text = text.Replace($"{exposedProperty.PropertyName}", exposedProperty.PropertyValue.ToString());
                newText = text.Replace("{", "").Replace("}", "");
            }
        }

        return newText;
    }

}