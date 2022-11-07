/*******************************************************************************************
*
*    File: Inventory.cs
*    Purpose: An Interactable player inventory
*    Author: Sam Blakely
*    Date: 24/10/2022
*    Updated: 2/11/2022
*
**********************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Character
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private Transform inventorySlotContainer;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Button combineButton;
        [SerializeField] private Button useButton;
        [SerializeField] private RecipeDataBase recipeDataBase;

        private List<InventorySlot> selectedslots = new List<InventorySlot>();
        private int slotCount;
        public List<Item> items { get; private set; } = new List<Item>();
        private List<InventorySlot> slots = new List<InventorySlot>();
        private CharacterController player;
        private void InventorySlot_OnSlotClick(InventorySlot obj)
        {
            if (selectedslots.Contains(obj))
            {
                obj.buttonObj.image.color = obj.buttonObj.colors.normalColor;
                selectedslots.Remove(obj);
            }
            else
            {
                obj.buttonObj.image.color = obj.buttonObj.colors.selectedColor;
                selectedslots.Add(obj);
            }
        }
        
        private void OnEnable()
        {
            player = FindObjectOfType<CharacterController>();
            if (player)
            {
                player.ToggleMovement(false);
                //player.ToggleSanity(false);
            }

            foreach (var slot in slots)
            {
                var slotObj = slot.GetComponent<InventorySlot>();
                slotObj.SlotClicked.AddListener(InventorySlot_OnSlotClick);
            }
            ShowSlots();
        }
        private void OnDisable()
        {
            foreach (var slot in selectedslots)
            {
                if (slot != null)
                {
                    slot.buttonObj.image.color = slot.buttonObj.colors.normalColor;
                }
                
            }
            
            foreach (var slot in slots)
            {
                var slotObj = GetComponent<InventorySlot>();
                slotObj.SlotClicked.RemoveListener(InventorySlot_OnSlotClick);
                if (slot != null)
                {
                    slot.gameObject.SetActive(false);
                }
            }
            slotCount = items.Count;
            if (player)
            {
                player.ToggleMovement(true);
                //player.ToggleSanity(true);
            }
        }

        private void ShowSlots()
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
                CreateSlot(items[i]);
            }
        }

        private void CreateSlot(Item item)
        {
            var slot = Instantiate(slotPrefab, inventorySlotContainer);
            var slotObj = slot.GetComponent<InventorySlot>();
            slotObj.SetItem(item);
            slots.Add(slotObj);
        }

        public void AddToInventory(Item item, bool inventoryIsOpen = false)
        {
            items.Add(item);
        }

        public bool HasItem(int id)
        {
            return items.Exists(x => x.itemID == id);
        }

        public bool HasItem(Item item)
        {
            return HasItem(item.itemID);
        }

        public void UseItem(int id)
        {
            var item = items.Find(x => x.itemID == id);
            var slot = slots.Find(x => x.GetItem.itemID == item.itemID);
            if (slot != null)
            {
                Destroy(slot.gameObject);
                slotCount = slots.Count;
            }
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

        public void CombineButtonClick()
        {
            if (selectedslots.Count > 1)
            {
                List<Item> checkList = selectedslots.Select(x => x.GetItem).ToList();
                foreach (var recipe in recipeDataBase.recipes)
                {
                    if (recipe.RecipeConditionsMet(checkList))
                    {
                        if (selectedslots.Any(x=> !x.GetItem.disappearsOnCombination))
                        {
                            var questObject = selectedslots.FindIndex(x => !x.GetItem.disappearsOnCombination);
                            var slotsToDestroy = new List<InventorySlot>();
                            for (int i = 0; i < selectedslots.Count; i++)
                            {
                                if (i != questObject)
                                {
                                    selectedslots[questObject].GetItem.Buff(selectedslots[i].GetItem);
                                    if (selectedslots[i].GetItem.isSingleUse)
                                    {
                                        slotsToDestroy.Add(selectedslots[i]);
                                    }
                                }
                            }
                            CleanupInventory(slotsToDestroy);
                            return;
                        }
                        AddToInventory(recipe.result);
                        CleanupInventory(selectedslots);
                        CreateSlot(recipe.result);
                    }
                }
            }
        }

        private void CleanupInventory(List<InventorySlot> inventorySlots)
        {
            foreach (var slot in inventorySlots)
            {
                items.Remove(slot.GetItem);
                slots.Remove(slot);
                Destroy(slot.gameObject);
            }
            inventorySlots.Clear();
        }
    }
}