using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC2_ChoiceHandler : ChoiceHandler
{
    [SerializeField] private GameObject door;
    
    // Handle the choices made in the conversation
    public override void HandleChoice(NodeLinkData choice) {
        // Open Door
        if (choice.TargetNodeGuid == "33d511c6-403c-4a47-ade0-d78aec74e5ab") {
            DoorState state = door.GetComponent<DoorState>();
            state.Open();
        }
    }
}
