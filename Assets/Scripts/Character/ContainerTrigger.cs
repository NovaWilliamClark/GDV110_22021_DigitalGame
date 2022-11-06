using Unity.VisualScripting;
using UnityEngine;

public class ContainerTrigger : InteractionPoint
{
    [SerializeField] private ContainerInventory containerInventory;
    [SerializeField] private InteractiveData data;


    protected override void Interact(CharacterController cc)
    {
        containerInventory.Init(cc);
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
        gameObject.SetActive(false);
    }
}