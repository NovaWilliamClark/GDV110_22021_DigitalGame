/*******************************************************************************************
*
*    File: Inventory.cs
*    Purpose: An Interactable player inventory
*    Author: Sam Blakely
*    Date: 24/10/2022
*    Updated: 
*
**********************************************************************************************/

using System;
using System.Collections.Generic;
using Objects;
using UnityEngine;

namespace Character
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private Transform inventorySlotContainer;
        [SerializeField] private GameObject slotPrefab;
        private int slotCount;
        public List<Item> items { get; private set; } = new List<Item>();
        private List<InventorySlot> slots = new List<InventorySlot>();
        private void OnEnable()
        {
            foreach (var slot in slots)
            {
                if (slot != null)
                {
                    slot.gameObject.SetActive(true);
                }
            }
            for (int i = slotCount; i < items.Count; i++)
            {
                var slot = GameObject.Instantiate(slotPrefab, inventorySlotContainer);
                var slotObj = slot.GetComponent<InventorySlot>();
                slotObj.SetItem(items[i]);
                slots.Add(slotObj);
            }
            
        }

        private void OnDisable()
        {
            foreach (var slot in slots)
            {
                if (slot != null)
                {
                    slot.gameObject.SetActive(false);
                }
            }
            slotCount = items.Count;
        }

        public void AddToInventory(Item item)
        {
            items.Add(item);
        }

        public bool HasItem(int id)
        {
            return items.Exists(x => x.itemID == id);
        }

        public void UseItem(int id)
        {
            var item = items.Find(x => x.itemID == id);
            var slot = slots.Find(x => x.GetItem.itemID == item.itemID);
            Destroy(slot.gameObject);
            slotCount = slots.Count;
            items.Remove(item);
        }

        public void CloseInventory()
        {
            gameObject.SetActive(false);
        }

        public void Init(List<Item> playerDataInventoryItems)
        {
            items = playerDataInventoryItems;
            slotCount = 0;
        }
    }
}
