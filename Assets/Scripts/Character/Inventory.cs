/*******************************************************************************************
*
*    File: Inventory.cs
*    Purpose: An Interactable player inventory
*    Author: Sam Blakely
*    Date: 24/10/2022
*    Updated: 2/11/2022
*
**********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] private RectTransform container;

        [HideInInspector] public UnityEvent<ItemData> ItemAdded;

        private List<InventorySlot> selectedslots = new List<InventorySlot>();
        private int slotCount;
        public List<ItemData> items { get; private set; } = new List<ItemData>();
        private List<InventorySlot> slots = new List<InventorySlot>();
        private CharacterController player;
        private CanvasGroup canvasGroup;

        [Header("Visuals")] [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private float moveDuration;
        [SerializeField] private Ease easeType;
        
        private void InventorySlot_OnSlotClick(InventorySlot obj)
        {
            Debug.Log(obj);
            if (selectedslots.Contains(obj))
            {
                selectedslots.Remove(obj);
            }
            else
            {
                selectedslots.Add(obj);
            }
        }
        
        private void OnEnable()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            player = FindObjectOfType<CharacterController>();
            canvasGroup.interactable = false;
            if (player)
            {
                player.ToggleMovement(false);
                //player.ToggleSanity(false);
            }

            if (slots.Count <= 0)
            {
                slots = GetComponentsInChildren<InventorySlot>().ToList();
            }

            foreach (var slot in slots)
            {
                slot.SlotClicked.AddListener(InventorySlot_OnSlotClick);
            }
            
            useButton.onClick.AddListener(UseButtonClicked);
            //ShowSlots();
            Cursor.visible = true;
            player.Equipment.DisableInput();
            startPosition = container.anchoredPosition;
            container.anchoredPosition = endPosition;
            container.DOAnchorPos(startPosition, moveDuration).SetEase(easeType).OnComplete(() =>
            {
                canvasGroup.interactable = true;
            });
        }

        private void UseButtonClicked()
        {
            UseItem();
        }

        private void OnDisable()
        {
            useButton.onClick.RemoveListener(UseButtonClicked);
            selectedslots.Clear();
            
            foreach (var slot in slots)
            {

                if (slot != null)
                {
                    slot.SlotClicked.RemoveAllListeners();
                    //slot.gameObject.SetActive(false);
                }
            }
            slotCount = items.Count;
            if (player)
            {
                player.ToggleMovement(true);
                //player.ToggleSanity(true);
            }
            
            Cursor.visible = false;
            player.Equipment.EnableInput();
        }

        // private void ShowSlots()
        // {
        //     foreach (var slot in slots)
        //     {
        //         if (slot != null)
        //         {
        //             slot.gameObject.SetActive(true);
        //         }
        //     }
        //     for (int i = slotCount; i < items.Count; i++)
        //     {
        //         CreateSlot(items[i]);
        //     }
        // }

        private void CreateSlot(ItemData itemData)
        {
            var slot = Instantiate(slotPrefab, inventorySlotContainer);
            var slotObj = slot.GetComponent<InventorySlot>();
            slotObj.SlotClicked.AddListener(InventorySlot_OnSlotClick);
            slotObj.SetItem(itemData);
            slots.Add(slotObj);
        }

        public void AddToInventory(ItemData itemData, bool inventoryIsOpen = false)
        {
            //items.Add(item);
            ItemAdded?.Invoke(itemData);
            items.Add(ItemData.CreateInstance(itemData));
        }

        public bool HasItem(int id)
        {
            return items.Exists(x => x.itemID == id);
        }

        public bool HasItem(ItemData itemDataOriginal)
        {
            return items.Exists(x => x.IsInstanceOf(itemDataOriginal));
        }

        public void UseItem(ItemData itemDataRef = null)
        {
            if (itemDataRef)
            {
                var item = items.Find(i => i.IsInstanceOf(itemDataRef));
                if (item)
                {
                    item.Use();
                    if (item.isSingleUse)
                    {
                        items.Remove(item);
                    }
                    if (slots.Count > 0)
                    {
                        var slot = slots.Find(s => s.GetItemData == item);
                        if (slot)
                        {
                            slots.Remove(slot);
                            Destroy(slot.gameObject);
                            slotCount = slots.Count;
                        }
                    }
                }
            }
            if (selectedslots.Count == 1)
            {
                var item = selectedslots[0].GetItemData;
                var slot = slots.Find(x => x.GetItemData.itemID == item.itemID);
                if (item.isSingleUse)
                {
                    if (slot != null)
                    {
                        if (item.isSingleUse)
                        {
                            slots.Remove(slot);
                            Destroy(slot.gameObject);
                            slotCount = slots.Count;
                        }
                    }
                    items.Remove(item);
                }

                if (slot != null)
                {
                    slot.SlotClick();
                }

                var effect = ItemDatabase.Instance.GetItemEffect(item);
                if (effect != null)
                {
                    effect.Use();
                }
            }
        }

        public void UseItem(int id)
        {
            var item = items.Find(i => i.itemID == id);
            if (item)
            {
                UseItem(item);
            }
        }
        
        public void CloseInventory()
        {
            gameObject.SetActive(false);
        }

        public void Init(List<ItemData> playerDataInventoryItems)
        {
            items = playerDataInventoryItems;
            slotCount = 0;
        }

        public void CombineButtonClick()
        {
            if (selectedslots.Count > 1)
            {
                List<ItemData> checkList = selectedslots.Select(x => x.GetItemData).ToList();
                foreach (var recipe in recipeDataBase.recipes)
                {
                    if (recipe.RecipeConditionsMet(checkList))
                    {
                        if (selectedslots.Any(x=> !x.GetItemData.disappearsOnCombination))
                        {
                            var questObject = selectedslots.FindIndex(x => !x.GetItemData.disappearsOnCombination);
                            var slotsToDestroy = new List<InventorySlot>();
                            for (int i = 0; i < selectedslots.Count; i++)
                            {
                                if (i != questObject)
                                {
                                    selectedslots[questObject].GetItemData.Buff(selectedslots[i].GetItemData);
                                    if (selectedslots[i].GetItemData.isSingleUse)
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

        private void Update()
        {
            useButton.interactable = selectedslots.Count == 1;
        }

        private void CleanupInventory(List<InventorySlot> inventorySlots)
        {
            foreach (var slot in inventorySlots)
            {
                items.Remove(slot.GetItemData);
                slots.Remove(slot);
                Destroy(slot.gameObject);
            }
            inventorySlots.Clear();
        }
    }
}