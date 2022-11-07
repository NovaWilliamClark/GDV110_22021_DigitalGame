using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ContainerTrigger : InteractionPoint
{
    [SerializeField] private ContainerInventory containerInventory;
    [SerializeField] private InteractiveData data;

    protected override void Interact(CharacterController cc)
    {
        containerInventory.Init(cc);
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
        gameObject.SetActive(false);
    }
}