using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    [SerializeField] private GameObject dialogueManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GameObject().CompareTag("Player"))
        {
            dialogueManager.GetComponent<WorldDialogueManager>().StartDialogue(dialogue);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GameObject().CompareTag("Player"))
        {
            dialogueManager.GetComponent<WorldDialogueManager>().EndDialogue();
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (var pos in dialogue.positions)
        {
            Gizmos.DrawWireCube(pos, Vector3.one);
        }
    }
}
