using UnityEngine;

public class ChoiceHandler : MonoBehaviour
{
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;

    // Handle the choices made in the conversation
    public void HandleChoice(NodeLinkData choice) {
        // Open left door
        if (choice.TargetNodeGuid == "a2de3242-5b04-42a0-bd34-1b396df3f460") {
            DoorState state = doorLeft.GetComponent<DoorState>();
            state.Open();
        }

        // Open right door
        if (choice.TargetNodeGuid == "315d4bef-8534-458e-82e8-dbf8ca16eb50") {
            DoorState state = doorRight.GetComponent<DoorState>();
            state.Open();
        }
    }
}
