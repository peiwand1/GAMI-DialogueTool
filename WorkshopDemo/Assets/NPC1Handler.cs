using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC1Handler : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private DialogueContainer dialogue;
    private DialogueParser parser;
    private bool isSpeaking;

    // Start is called before the first frame update
    void Start()
    {
        parser = gameObject.GetComponent<DialogueParser>();
    }

    // Call this when you want to start a conversation
    void StartDialogue(DialogueContainer dialogue) {
        isSpeaking = true;
        parser.StartDialogue(dialogue);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            parser.EnableDialogue();
            if (!isSpeaking) {
                StartDialogue(dialogue);
            } 
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            parser.DisableDialogue();
        }
    }
}
