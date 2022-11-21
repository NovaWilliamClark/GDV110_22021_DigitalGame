using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : InteractionPoint
{
    [Header("Dialogue")]
    public Dialogue dialogue;

    private bool hasStarted = false;

    private WorldDialogue box;

    protected override void Awake()
    {
        base.Awake();
        canInteract = false;
        levelController.PlayerSpawned.AddListener(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(CharacterController cc)
    {
        playerRef = cc;
        canInteract = true;
        levelController.PlayerSpawned.RemoveListener(OnPlayerSpawned);
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        if (!hasItem) return;
        if (collision.GameObject().CompareTag("Player"))
        {
            box.End();
            hasStarted = false;
            if (canReInteract)
            {
                hasInteracted = false;
            }
            Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
        }
    }

    protected override void Interact(CharacterController cc)
    {
        if (requiresItem)
        {
            hasItem = cc.GetInventory.HasItem(requiredItem);
            if (!hasItem)
            {
                hasInteracted = false;
                return;
            }
        }
        if (!hasStarted)
        {
            hasStarted = true;
            foreach (var d in dialogue.entries)
            {
                d.position = transform.position + d.position;
            }
            box = WorldDialogueManager.Instance.CreateDialogueBox(dialogue);
            box.gameObject.SetActive(true);
            box.StartDialogue();
            box.Completed.AddListener(OnDialogueComplete);
        }
        else
        {
            if (!box.IsCompleted)
            {
                box.Resume();
            }
        }
    }

    private void OnDialogueComplete(WorldDialogue arg0)
    {
        Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
    }

    private void OnDrawGizmosSelected()
    {
        if (dialogue.entries.Count == 0) return;
        foreach (var pos in dialogue.entries)
        {
            
            Gizmos.DrawWireCube(transform.position + pos.position, Vector3.one);
        }
    }

    public override void SetInteractedState(object state)
    {
        base.SetInteractedState(state);
        gameObject.SetActive(false);
    }

    public void DisableFromTrigger()
    {
        Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
    }
}
    