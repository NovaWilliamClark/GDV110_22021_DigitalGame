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

public class InteractableSwitch : MonoBehaviour
{
    [SerializeField] private GameObject promptMessage;
    [SerializeField] private GameObject resultGameObject;
    [SerializeField] private Collider2D collider;
     private bool canInteract = true;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canInteract)
        {
            promptMessage.GetComponentInChildren<TMP_Text>().text = "Interact - 'E'";
            promptMessage.SetActive(true);
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
        promptMessage.SetActive(false);
    }

    private void Interact()
    {
        promptMessage.SetActive(false);
        resultGameObject.SetActive(true);
    }
}