/*******************************************************************************************
*
*    File: InteractionPoint.cs
*    Purpose: Abstract base class for object and environment interactions
*    Author: Sam Blakely
*    Date: 11/10/2022
*    Updated: 24/10/2022
*
**********************************************************************************************/


using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class InteractionPoint : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] protected GameObject promptBox;
    [SerializeField] private string promptMessage;

    private BoxCollider2D triggerArea;
    private bool canInteract = true;

    private PlayerInput input;

     private void Awake()
     {
         triggerArea = GetComponent<BoxCollider2D>();
         if (triggerArea)
         {
             triggerArea.isTrigger = true;
         }
     }

     private void Start()
     {
         input = new PlayerInput();
         input.Enable();
     }

     private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<CharacterController>()) return;
        if (!canInteract) return;
        if (!promptBox) return;
        
        promptBox.GetComponentInChildren<TMP_Text>().text = promptMessage;
        promptBox.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.GetComponent<CharacterController>()) return;
        //if (!(Input.GetButton("Interact"))) return;
        if (!input.Player.Interact.IsPressed()) return;
        Interact(other.GetComponent<CharacterController>());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.GetComponent<CharacterController>()) return;
        DisablePrompt();
    }

    protected void DisableInteraction()
    {
        DisablePrompt();
        canInteract = false;
        triggerArea.enabled = false;
    }

    private void DisablePrompt()
    {
        if (promptBox)
        {
            promptBox.SetActive(false);
        }
    }

    protected abstract void Interact(CharacterController cc);
}