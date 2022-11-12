using Audio;
using Objects;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ContainerTrigger : InteractionPoint
{
    [SerializeField] private ContainerInventory containerInventory;

    [SerializeField] private bool requiresItem;
    [SerializeField] private ItemData item;
    private bool hasItem;

    [SerializeField] private string missingItemMessage;
    
    protected override void Interact(CharacterController cc)
    {
        if (requiresItem)
        {
            hasItem = playerRef.GetInventory.HasItem(item);
            if (!hasItem)
            {
                hasInteracted = false;
                return;
            }
            AudioManager.Instance.PlaySound(item.useSfx, item.useSfxVolume);
            playerRef.GetInventory.UseItem(item);
        }
        
        AudioManager.Instance.PlaySound(useSfx, volume);
        containerInventory.Init(cc);
        DisablePrompt();
        hasInteracted = true;
        //containerInventory.gameObject.SetActive(true);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!automaticInteraction)
        {
            if (!other.GetComponent<CharacterController>()) return;
            if (!canInteract) return;
            if (!promptBox) return;
            
            playerRef = other.GetComponent<CharacterController>();

            hasItem = playerRef.GetInventory.HasItem(item);
            var msg = !hasItem ? missingItemMessage : promptMessage;
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
    
    protected override void Start()
    {
        base.Start();
        containerInventory.OnContainerEmptied += ContainerInventory_OnContainerEmptied;
    }

    private void ContainerInventory_OnContainerEmptied()
    {
        canInteract = false;
        hasInteracted = true;
        DisablePrompt();
        //gameObject.SetActive(false);
    }
}