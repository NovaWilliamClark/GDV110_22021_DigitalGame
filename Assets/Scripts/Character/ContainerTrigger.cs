using Audio;
using Objects;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ContainerTrigger : InteractionPoint
{
    [SerializeField] private ContainerInventory containerInventory;

    [SerializeField] private string missingItemMessage;
    
    protected override void Interact(CharacterController cc)
    {
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
            hasInteracted = false;
            ResetPrompt(promptMessage);
        }
        else
        {
            canInteract = false;
            hasInteracted = true;
            Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
        }
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