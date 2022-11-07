using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ContainerTrigger : InteractionPoint
{
    [SerializeField] private ContainerInventory containerInventory;
    [SerializeField] private InteractiveData data;

    protected override void Interact(CharacterController cc)
    {
        containerInventory.Init(cc);
        DisablePrompt();
        hasInteracted = false;
        containerInventory.gameObject.SetActive(true);
    }

    protected override void Start()
    {
        base.Start();
        containerInventory.OnContainerEmptied += ContainerInventory_OnContainerEmptied;
    }

    private void ContainerInventory_OnContainerEmptied()
    {
        data.state = InteractiveData.InteractionState.INACTIVE;
        canInteract = false;
        hasInteracted = true;
        DisablePrompt();
        //gameObject.SetActive(false);
    }
}