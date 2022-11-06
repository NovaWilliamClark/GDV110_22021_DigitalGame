using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ContainerTrigger : InteractionPoint
{
    [SerializeField] private ContainerInventory containerInventory;
    [SerializeField] private InteractiveData data;
    [SerializeField] private Material outlineMaterial;

    private bool inRange = false;
    [SerializeField] private float fxRadius = 10f;

    protected override void Awake()
    {
        base.Awake();
        renderer = GetComponent<SpriteRenderer>();
        outlineMaterial = renderer.material;
        outlineMaterial.SetFloat("_varTime", 0f);
    }

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