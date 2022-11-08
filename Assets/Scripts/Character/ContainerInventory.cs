using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using UnityEngine;

public class ContainerInventory : MonoBehaviour
{
    public event Action OnContainerEmptied;
    
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private ItemContainer_SO inventoryItems;
    [SerializeField] private Transform slotContainer;
    private List<GameObject> slots = new List<GameObject>();
    private List<InventorySlot> selectedSlots = new List<InventorySlot>(); 
    private CharacterController player;
    private bool accessed = false;

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        foreach (var item in inventoryItems.Items)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            var slotObj = slot.GetComponent<InventorySlot>();
            slotObj.SlotClicked.AddListener(InventorySlot_OnSlotClick);
            slotObj.SetItem(item);
            slots.Add(slot);
        }
    }

    private void InventorySlot_OnSlotClick(InventorySlot obj)
    {
        Debug.Log(obj);
        if (selectedSlots.Contains(obj))
        {
            selectedSlots.Remove(obj);
        }
        else
        {
            selectedSlots.Add(obj);
        }
    }

    private void OnDisable()
    {
        foreach (var slot in slots)
        {
            var slotObj = slot.GetComponent<InventorySlot>();
            slotObj.SlotClicked.RemoveListener(InventorySlot_OnSlotClick);
            Destroy(gameObject);
        }
    }

    public void Init(CharacterController cc)
    {
        player = cc;
        if (!accessed)
        {
            inventoryItems.Init();
            accessed = true;
        }
        inventoryItems.SetItems();
    }

    public void TakeAll()
    {
        foreach (var item in inventoryItems.Items)
        {
            player.GetInventory.AddToInventory(item);
            inventoryItems.SetToTaken(item.itemID);
        }
        OnContainerEmptied?.Invoke();
        gameObject.SetActive(false);
    }

    public void TakeSelected()
    {
        foreach (var slot in selectedSlots.ToList())
        {
            player.GetInventory.AddToInventory(slot.GetItemData);
            inventoryItems.SetToTaken(slot.GetItemData.itemID);
            selectedSlots.Remove(slot);
            slots.Remove(slot.gameObject);
            Destroy(slot.gameObject);
        }

        if (slots.Count == 0)
        {
            OnContainerEmptied?.Invoke();
            gameObject.SetActive(false);
        }
    }
}