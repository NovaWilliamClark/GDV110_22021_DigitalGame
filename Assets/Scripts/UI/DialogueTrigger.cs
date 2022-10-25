using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    [SerializeField] private GameObject dialoguePrefab;

    private GameObject prefabInstance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GameObject().CompareTag("Player"))
        {
            prefabInstance = Instantiate(dialoguePrefab, Vector3.zero, Quaternion.identity);
            prefabInstance.GetComponent<WorldDialogueManager>().StartDialogue(dialogue);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GameObject().CompareTag("Player"))
        {
            prefabInstance.GetComponent<WorldDialogueManager>().EndDialogue();
        }
    }
}
