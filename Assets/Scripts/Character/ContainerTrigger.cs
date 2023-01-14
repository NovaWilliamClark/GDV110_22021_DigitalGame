using Audio;
using Objects;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ContainerTrigger : InteractionPoint
{
    [SerializeField] private ContainerInventory containerInventory;

    [SerializeField] private string missingItemMessage;

    protected bool containerEmptied = false;

    protected override void Awake()
    {
        base.Awake();
        containerInventory.Ready.AddListener(OnContainerInventoryReady);
    }

    private void OnContainerInventoryReady()
    {
        if (containerInventory.Emptied)
        {
            containerEmptied = true;
        }
        containerInventory.Ready.RemoveListener(OnContainerInventoryReady);
    }

    protected override void Interact(CharacterController cc)
    {
        if (containerEmptied) return;
        if (requiresItem)
        {
            hasItem = playerRef.GetInventory.HasItem(requiredItem);
            if (!hasItem)
            {
                hasInteracted = false;
                return;
            }
            AudioManager.Instance.PlaySound(requiredItem.useSfx, requiredItem.useSfxVolume);
            playerRef.GetInventory.UseItem(requiredItem);
        }
        
        AudioManager.Instance.PlaySound(useSfx, volume);
        containerInventory.Init(cc);
        playerRef = cc;
        cc.ToggleActive(false);
        containerInventory.ContainerClosed.AddListener(OnContainerClosed);
        DisablePrompt();
        hasInteracted = true;
        //containerInventory.gameObject.SetActive(true);
    }

    private void OnContainerClosed(bool emptied)
    {
        playerRef.ToggleActive(true);
        if (!emptied)
        {
            containerEmptied = false;
            hasInteracted = false;
            canInteract = true;
            ResetPrompt(promptMessage);
        }
        else
        {
            containerEmptied = true;
            canInteract = false;
            hasInteracted = true;
            Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
        }
        containerInventory.ContainerClosed.RemoveListener(OnContainerClosed);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!automaticInteraction)
        {
            if (!other.GetComponent<CharacterController>()) return;
            if (!canInteract) return;
            if (!promptBox) return;
            
            playerRef = other.GetComponent<CharacterController>();
            string msg;
            if (requiresItem)
            {
                hasItem = playerRef.GetInventory.HasItem(requiredItem);

                msg = !hasItem ? missingItemMessage : promptMessage;
            }
            else
            {
                msg = promptMessage;
            }
            promptBox.gameObject.SetActive(true);
            promptBox.Show(msg);
        }
        else
        {
            if (hasInteracted) return;
            Debug.Log("Auto Interaction!");
            hasInteracted = true;
            Interact(other.GetComponent<CharacterController>());
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (containerEmptied) return;
        if (!automaticInteraction)
        {
            if(!other.GetComponent<CharacterController>()) return;
            playerRef = null;
            DisablePrompt();
            
            if (canReInteract && !containerEmptied)
            {
                canInteract = true;
            }
            else
            {
                canInteract = false;
            }
        }
    }

    public override void SetInteractedState(object state)
    {
        base.SetInteractedState(state);
        if (state is InteractionState {interacted: true})
        {
            hasInteracted = true;
            canInteract = false;
        }
    }

    protected override void Start()
    {
        base.Start();
        //containerInventory.OnContainerEmptied += ContainerInventory_OnContainerEmptied;
    }

    private void ContainerInventory_OnContainerEmptied()
    {
        canInteract = false;
        hasInteracted = true;
        DisablePrompt();
        //gameObject.SetActive(false);
    }
}