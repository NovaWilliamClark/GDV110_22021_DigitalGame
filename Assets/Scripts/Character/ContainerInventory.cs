using System;
using System.Collections.Generic;
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
    private void OnEnable()
    {
        InventorySlot.OnSlotClick += InventorySlot_OnSlotClick;
        foreach (var item in inventoryItems.items)
        {
            var slot = Instantiate(slotPrefab, slotContainer);
            var slotObj = slot.GetComponent<InventorySlot>();
            slotObj.SetItem(item);
            slots.Add(slot);
        }
    }

    private void InventorySlot_OnSlotClick(InventorySlot obj)
    {
        if (selectedSlots.Contains(obj))
        {
            obj.buttonObj.image.color = obj.buttonObj.colors.normalColor;
            selectedSlots.Remove(obj);
        }
        else
        {
            obj.buttonObj.image.color = obj.buttonObj.colors.selectedColor;
            selectedSlots.Add(obj);
        }
    }

    private void OnDisable()
    {
        InventorySlot.OnSlotClick -= InventorySlot_OnSlotClick;
        foreach (var slot in slots)
        {
            Destroy(slot);
        }
    }

    public void Init(CharacterController cc)
    {
        player = cc;
        inventoryItems.SetItems();
    }

    public void TakeAll()
    {
        foreach (var item in inventoryItems.items)
        {
            player.GetInventory.AddToInventory(item);
            inventoryItems.SetToTaken(item.itemID);
        }
        OnContainerEmptied?.Invoke();
        gameObject.SetActive(false);
    }

    public void TakeSelected()
    {
        foreach (var slot in selectedSlots)
        {
            player.GetInventory.AddToInventory(slot.GetItem);
            inventoryItems.SetToTaken(slot.GetItem.itemID);
        }
    }
}