using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class Dumbwaitor : InteractionPoint
{
    [SerializeField] private PlayerData_SO playerData;
    [SerializeField] private string breakerBrokenMsg;

    [SerializeField] private Animator animator;
    
    protected override void Interact(CharacterController cc)
    {
        if (!cc.PlayerData.breakerFixed)
        {
            hasInteracted = false;
            return;
        }
        DisablePrompt();
        hasInteracted = true;
        if (animator)
        {
            animator.SetTrigger("Open");
        }
        
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
            if (!other.GetComponent<CharacterController>()) return;
            if (!canInteract) return;
            if (!promptBox) return;
            
            playerRef = other.GetComponent<CharacterController>();
            
            var msg = !playerData.breakerFixed ? breakerBrokenMsg : promptMessage;
            promptBox.gameObject.SetActive(true);
            promptBox.Show(msg);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
