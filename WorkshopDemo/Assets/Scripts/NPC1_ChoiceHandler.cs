using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC1_ChoiceHandler : ChoiceHandler
{
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;

    // Handle the choices made in the conversation
    public override void HandleChoice(NodeLinkData choice) {
        // Open left door
        if (choice.TargetNodeGuid == "a2de3242-5b04-42a0-bd34-1b396df3f460") {
            DoorState state = doorLeft.GetComponent<DoorState>();
            gameObject.GetComponent<NPCHandler>().dialogue.ExposedBooleanProperties.Find(x => x.PropertyName == choice.ConditionBoolean).PropertyValue = false;
            state.Open();
        }

        // Open right door
        if (choice.TargetNodeGuid == "315d4bef-8534-458e-82e8-dbf8ca16eb50") {
            DoorState state = doorRight.GetComponent<DoorState>();
            gameObject.GetComponent<NPCHandler>().dialogue.ExposedBooleanProperties.Find(x => x.PropertyName == choice.ConditionBoolean).PropertyValue = false;
            state.Open();
        }
    }
}
