using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using Objects;
using UnityEngine;
using UnityEngine.Events;

public class ContainerInventory : MonoBehaviour
{
    public event Action OnContainerEmptied;
    
    [SerializeField] private ItemContainer_SO inventoryItems;
    [SerializeField] private UIContainer containerPrefab;
    [SerializeField] private UIContainer container;
    private List<GameObject> slots = new List<GameObject>();
    private List<InventorySlot> selectedSlots = new List<InventorySlot>(); 
    private CharacterController player;
    private LevelController levelController;
    private bool accessed = false;
    private PersistentObject persistentObject;

    [HideInInspector] public UnityEvent<ContainerInventory, ItemContainerState> ContainerStateChanged;

    private void Awake()
    {
        persistentObject = GetComponent<PersistentObject>();
        
        levelController = FindObjectOfType<LevelController>();
        levelController.LevelInitialized.AddListener(OnLevelInitialized);
    }

    private void OnLevelInitialized()
    {
        container = Instantiate(containerPrefab);
        container.Setup(inventoryItems, TakeItem);
    }

    private void TakeItem(ItemData item)
    {
        player.GetInventory.AddToInventory(item);
        inventoryItems.Items.Remove(item);
    }

    public void Init(CharacterController cc)
    {
        player = cc;
        inventoryItems.Init();
        container.Closed.AddListener(OnContainerUIClosed);
        container.Open();
    }

    private void OnContainerUIClosed()
    {
        ContainerStateChanged?.Invoke(this, new ItemContainerState(persistentObject.Id) {items = inventoryItems.Items});
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
        foreach (var item in state.items)
        {
            inventoryItems.SetToTaken(item);
        }
    }
}