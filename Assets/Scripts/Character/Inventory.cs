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
using Audio;
using DG.Tweening;
using Objects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace Character
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private Transform inventorySlotContainer;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Button combineButton;
        [SerializeField] private Button useButton;
        [SerializeField] private Button reloadButton;
        [SerializeField] private RecipeDataBase recipeDataBase;
        [SerializeField] private RectTransform container;

        [HideInInspector] public UnityEvent<ItemData> ItemAdded;

        private List<InventorySlot> selectedslots = new List<InventorySlot>();
        private int slotCount;
        public List<ItemData> items { get; private set; } = new List<ItemData>();
        //public List<InventoryItem> items { get; private set; }
        public List<InventorySlot> slots = new List<InventorySlot>();
        private CharacterController player;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private CanvasGroup navGroup;

        [SerializeField] private ItemTypeEnum FlashlightType;

        [Header("Visuals")] [SerializeField] private Vector3 startPosition;
        [SerializeField] private Vector3 endPosition;
        [SerializeField] private float moveDuration;
        [SerializeField] private Ease easeType;
        private Sequence sequence;
        [SerializeField] private inventoryAnimation sockySlot;
        [SerializeField] private inventoryAnimation flashlightSlot;

        [Header("Audio")] 
        [SerializeField] private List<AudioClip> itemPickupSfx;
        [SerializeField] private List<AudioClip> openInventorySfx;

        private InventorySlot lastClickedSlot;
        private InventorySlot reloadableClicked;

        [Serializable]
        private struct inventoryAnimation
        {
            public RectTransform rect;
            public Vector3 startPosition;
            public Vector3 endPosition;
            public float duration;
            public Ease easeType;

            public Tween GetTween()
            {
                return rect.DOAnchorPos(startPosition, duration).SetEase(easeType);
            }
        }

        private void Awake()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            player = FindObjectOfType<CharacterController>();
        }

        private void Start()
        {
            startPosition = container.anchoredPosition;
            container.anchoredPosition = endPosition;

            navGroup.interactable = false;
            navGroup.alpha = 0;

            sockySlot.startPosition = sockySlot.rect.anchoredPosition;
            sockySlot.rect.anchoredPosition = sockySlot.endPosition;
            
            flashlightSlot.startPosition = flashlightSlot.rect.anchoredPosition;
            flashlightSlot.rect.anchoredPosition = flashlightSlot.endPosition;
            sequence = DOTween.Sequence();
            sequence
                .Append(container.DOAnchorPos(startPosition, moveDuration).SetEase(easeType))
                .Append(sockySlot.GetTween())
                .Insert(moveDuration + 0.2f, flashlightSlot.GetTween())
                .OnComplete(() =>
                {
                    canvasGroup.interactable = true;
                    foreach (var slot in slots)
                    {
                        
                        slot.buttonObj.interactable = true;
                    }

                    navGroup.DOFade(1f, .2f).OnComplete(() =>
                    {
                        navGroup.interactable = true;
                    });
                })
                .SetAutoKill(false);
            sequence.Pause();
        }

        private void InventorySlot_OnSlotClick(InventorySlot obj)
        {
            lastClickedSlot = obj;
            if (selectedslots.Contains(obj))
            {
                selectedslots.Remove(obj);
            }
            else
            {
                if (selectedslots.Count > 1)
                {
                    selectedslots[0].Deactivate();
                    selectedslots.RemoveAt(0);
                }
                selectedslots.Add(obj);
            }
        }
        
        private void OnEnable()
        {

        }

        public void OpenInventory()
        {
            player.Equipment.DisableInput();
            canvasGroup.interactable = false;
            if (player)
            {
                player.ToggleActive(false);
            }

            if (slots.Count <= 0)
            {
                slots = GetComponentsInChildren<InventorySlot>().ToList();
            }
            
            foreach (var slot in slots)
            {
                if (!HasItem(slot.itemReference))
                {
                    slot.gameObject.SetActive(false);
                    continue;
                }
                slot.gameObject.SetActive(true);
                slot.buttonObj.interactable = false;
                slot.SlotClicked.AddListener(InventorySlot_OnSlotClick);
                if (slot.itemReference.reloadable)
                {
                    slot.SlotClicked.AddListener(OnReloadableItemClicked);
                }
            }
            useButton.onClick.AddListener(UseButtonClicked);
            //ShowSlots();
            
            AudioManager.Instance.PlaySound(openInventorySfx[Random.Range(0,openInventorySfx.Count-1)]);
            
            Cursor.visible = true;
            
            sequence.PlayForward();
        }

        private void OnReloadableItemClicked(InventorySlot slot)
        {
            // enable reload button
            reloadButton.interactable = !reloadButton.interactable;
            reloadableClicked = slot;
            if (reloadButton.interactable)
            {
                reloadButton.onClick.AddListener(OnReloadButtonClicked);
            }
            else
            {
                reloadButton.onClick.RemoveListener(OnReloadButtonClicked);
            }
        }

        private void OnReloadButtonClicked()
        {
            //if 
            if (selectedslots.Count < 2)
            {
                Debug.Log("Other item not selected");
            }
            InventorySlot ammoSlot = null;
            foreach (var slot in selectedslots)
            {
                if (slot == reloadableClicked) continue;
                if (slot.itemReference == reloadableClicked.itemReference.itemToReload)
                {
                    ammoSlot = slot;
                    break;
                }
            }

            if (ammoSlot)
            {
                var itemRef = ammoSlot.itemReference;
                var item = items.First(x => x == x.IsInstanceOf(itemRef));
                var count = ItemCount(item);
                if (count == 0)
                {
                    Debug.LogWarningFormat("Trying to use ammo item: {0} to reload {1}, item not in inventory!",
                        ammoSlot.itemReference, reloadableClicked.itemReference);
                }

                if (count >= 1)
                {
                    items.Remove(item);
                    var effect = ItemDatabase.Instance.GetItemEffect(reloadableClicked.itemReference);
                    effect.Reload(FindObjectOfType<CharacterController>().gameObject);
                    if (count == 1)
                    {
                        selectedslots.Remove(ammoSlot);
                        AudioManager.Instance.PlaySound(ammoSlot.itemReference.reloadSfx);
                        ammoSlot.gameObject.SetActive(false);
                    }
                }
            }
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
                player.ToggleActive(true);
            }
            
            Cursor.visible = false;
            player.Equipment.EnableInput();
        }

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

        public int ItemCount(ItemData itemInstance)
        {
            var item = ItemDatabase.Instance.GetOriginalItem(itemInstance);
            var count = 0;
            if (item)
            {
                count += items.Count(data => data.IsInstanceOf(item));
            }

            return count;
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
                if (item && item.usableInInventory)
                {
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
                if (item.isSingleUse && item.usableInInventory)
                {
                    if (slot != null)
                    {
                        if (item.isSingleUse && item.usableInInventory)
                        {
                            slots.Remove(slot);
                            Destroy(slot.gameObject);
                            slotCount = slots.Count;
                        }
                    }
                    items.Remove(item);
                }

                var effect = ItemDatabase.Instance.GetItemEffect(item);
                if (effect != null)
                {
                    effect.Use(gameObject);
                }

                if (item.usableInInventory)
                {
                    selectedslots.Remove(slot);
                    slot.Deactivate();
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
            canvasGroup.interactable = false;
            navGroup.interactable = false;
            navGroup.DOFade(0f, .2f);
            sequence.OnRewind(
                () =>
                {
                    canvasGroup.interactable = true;
                    
                    foreach (var slot in slots)
                    {
                        slot.buttonObj.interactable = false;
                        slot.SlotClicked.RemoveAllListeners();
                        slot.SlotClicked.RemoveAllListeners();
                    }
                    Cursor.visible = false;
                    player.ToggleActive(true);
                }).PlayBackwards();
            
            useButton.onClick.RemoveListener(UseButtonClicked);
            selectedslots.Clear();

            Cursor.visible = false;
            player.Equipment.EnableInput();
        }

        private string WaitForHide()
        {
            throw new NotImplementedException();
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