/*******************************************************************************************
*
*    File: InteractionPoint.cs
*    Purpose: Abstract base class for object and environment interactions
*    Author: Sam Blakely
*    Date: 11/10/2022
*    Updated: 24/10/2022
*
**********************************************************************************************/

using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;

[RequireComponent(typeof(BoxCollider2D))]
public abstract class InteractionPoint : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] protected GameObject promptBox;
    [SerializeField] protected string promptMessage;

    [SerializeField] protected bool automaticInteraction = false;
    private BoxCollider2D triggerArea;
    private bool canInteract = true;

    private PlayerInput input;

     protected virtual void Awake()
     {
         triggerArea = GetComponent<BoxCollider2D>();
         if (triggerArea)
         {
             triggerArea.isTrigger = true;
         }
     }

     protected virtual void Start()
     {
         input = new PlayerInput();
         input.Enable();
     }

     protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!automaticInteraction)
        {
            if (!other.GetComponent<CharacterController>()) return;
            if (!canInteract) return;
            if (!promptBox) return;
        
            promptBox.GetComponentInChildren<TMP_Text>().text = promptMessage;
            promptBox.SetActive(true);
        }
        else
        {
            Debug.Log("Auto Interaction!");
            Interact(other.GetComponent<CharacterController>());
        }
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!automaticInteraction)
        {
            if (!other.GetComponent<CharacterController>()) return;
            //if (!(Input.GetButton("Interact"))) return;
            if (!input.Player.Interact.IsPressed()) return;
            Interact(other.GetComponent<CharacterController>());
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!automaticInteraction)
        {
            if(!other.GetComponent<CharacterController>()) return;
            DisablePrompt();
        }
    }

    // protected void DisableInteraction()
    // {
    //     DisablePrompt();
    //     canInteract = false;
    //     triggerArea.enabled = false;
    // }

    private void DisablePrompt()
    {
        if (promptBox)
        {
            promptBox.SetActive(false);
        }
    }

    protected abstract void Interact(CharacterController cc);
}