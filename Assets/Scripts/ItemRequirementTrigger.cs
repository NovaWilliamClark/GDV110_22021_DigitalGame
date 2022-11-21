using System;
using System.Collections;
using System.Collections.Generic;
using Objects;
using UnityEngine;
using UnityEngine.Events;

public class ItemRequirementTrigger : GenericObject
{
    [SerializeField] private ItemData requiredItem;

    public UnityEvent RequirementSatisfied;
    private LevelController controller;
    private CharacterController player;
    private void Awake()
    {
        controller = FindObjectOfType<LevelController>();
        controller.PlayerSpawned.AddListener(OnPlayerSpawned);
    }

    private void OnPlayerSpawned(CharacterController cc)
    {
        player = cc;
        player.GetInventory.ItemAdded.AddListener(OnItemAdded);
    }

    private void OnItemAdded(ItemData item)
    {
        if (item == requiredItem)
        {
            player.GetInventory.ItemAdded.RemoveListener(OnItemAdded);
            ObjectStateChanged?.Invoke(this, new GenericState(GetComponent<PersistentObject>().Id){active = false});
            RequirementSatisfied?.Invoke();
        }
    }

    public override void SetPersistentState(GenericState state)
    {
        RequirementSatisfied.Invoke();
        base.SetPersistentState(state);
    }
}
