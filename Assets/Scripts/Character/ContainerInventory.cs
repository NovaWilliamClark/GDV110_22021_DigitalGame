﻿using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using Objects;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PersistentObject))]
public class ContainerInventory : MonoBehaviour
{
    public event Action OnContainerEmptied;
    
    [SerializeField] private ItemContainer_SO inventoryItems;
    [SerializeField] private GameObject containerPrefab;
    [SerializeField] private UIContainer container;
    private List<GameObject> slots = new List<GameObject>();
    private List<InventorySlot> selectedSlots = new List<InventorySlot>(); 
    private CharacterController player;
    private LevelController levelController;
    private bool accessed = false;
    private PersistentObject persistentObject;

    [HideInInspector] public UnityEvent<ContainerInventory, ItemContainerState> ContainerStateChanged;
    public UnityEvent<bool> ContainerClosed;
 
    private void Awake()
    {
        persistentObject = GetComponent<PersistentObject>();
        
        levelController = FindObjectOfType<LevelController>();
        levelController.LevelInitialized.AddListener(OnLevelInitialized);
    }

    private void OnLevelInitialized()
    {
        container = Instantiate(containerPrefab).GetComponentInChildren<UIContainer>();
        container.Setup(inventoryItems, TakeItem);
        container.gameObject.SetActive(false);
    }

    private void TakeItem(ItemData item)
    {
        player.GetInventory.AddToInventory(item);
        inventoryItems.SetToTaken(item);
        //inventoryItems.Items.Remove(item);
    }

    public void Init(CharacterController cc, UnityAction closedCallback = null)
    {
        player = cc;
        if (!accessed)
        {
            inventoryItems.Init();
            accessed = true;
        }
        container.gameObject.SetActive(true);
        container.Closed.AddListener(OnContainerUIClosed);
        container.ItemClicked.AddListener(TakeItem);
        container.Open();
    }

    private void OnContainerUIClosed()
    {
        container.gameObject.SetActive(false);
        ContainerClosed?.Invoke(inventoryItems.AllItemsTaken);
        ContainerStateChanged?.Invoke(this, new ItemContainerState(persistentObject.Id) {items = inventoryItems.ItemsTaken.ToList()});
    }

    public void SpawnItems()
    {
        foreach (var item in inventoryItems.Items)
        {
            Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
        }
        OnContainerEmptied?.Invoke();
    }

    public void SetContainerState(ItemContainerState state)
    {
        inventoryItems.Init();
        foreach (var item in state.items.ToList())
        {
            inventoryItems.SetToTaken(item);
        }
        accessed = true;
    }
}