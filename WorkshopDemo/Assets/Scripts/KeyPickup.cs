using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private DialogueContainer dialogue;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            dialogue.ExposedBooleanProperties.Find(x => x.PropertyName == "HasKey").PropertyValue = true;
            gameObject.SetActive(false);
        }
    }
}
