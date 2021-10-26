using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC2_ChoiceHandler : ChoiceHandler
{
    [SerializeField] private GameObject middleDoor;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    
    // Handle the choices made in the conversation
    public override void HandleChoice(NodeLinkData choice) {
        // Open Middle Door
        if (choice.TargetNodeGuid == "33d511c6-403c-4a47-ade0-d78aec74e5ab") {
            DoorState state = middleDoor.GetComponent<DoorState>();
            state.Open();
        }

        // Open Left Door
        if (choice.TargetNodeGuid == "") {
            DoorState state = leftDoor.GetComponent<DoorState>();
            state.Open();     
        }

        // Open Right Door
        if (choice.TargetNodeGuid == "") {
            DoorState state = rightDoor.GetComponent<DoorState>();
            state.Open();
        }
    }
}
