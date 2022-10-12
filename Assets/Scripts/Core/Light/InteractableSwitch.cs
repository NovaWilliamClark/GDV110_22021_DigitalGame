/*******************************************************************************************
*
*    File: InteractableSwitch.cs
*    Purpose: Provides a player interactable switch and prompt
*    Author: Sam Blakely
*    Date: 11/10/2022
*
**********************************************************************************************/


using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class InteractableSwitch : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private GameObject promptBox;
    [SerializeField] private string promptMessage;
    
    [Header("Result Activation")]
    [SerializeField] private GameObject objectToActivate;
    
     private BoxCollider2D collider;
     private bool canInteract = true;

     private void Awake()
     {
         collider = GetComponent<BoxCollider2D>();
         if (collider)
         {
             collider.isTrigger = true;
         }
     }

     private void OnTriggerEnter2D(Collider2D other)
    {
        if (canInteract)
        {
            if (promptBox)
            {
                promptBox.GetComponentInChildren<TMP_Text>().text = promptMessage;
                promptBox.SetActive(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!(Input.GetButton("Interact"))) return;
        Interact();
        canInteract = false;
        collider.enabled = false;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (promptBox)
        {
            promptBox.SetActive(false);
        }
    }

    private void Interact()
    {
        if (promptBox)
        {
            promptBox.SetActive(false);
        }
        
        if (objectToActivate)
        {
            objectToActivate.SetActive(true);
        }
    }
}