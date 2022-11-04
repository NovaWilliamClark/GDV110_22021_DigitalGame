/*******************************************************************************************
*
*    File: InventorySlot.cs
*    Purpose: Represents a slot in the inventory to hold an item.
*    Author: Sam Blakely
*    Date: 24/10/2022
*    Updated: 
*
**********************************************************************************************/

using System;
using Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    [Serializable]
    public class InventorySlot : MonoBehaviour
    {
        public static event Action<InventorySlot> OnSlotClick; 
        [SerializeField] private TextMeshProUGUI slotText;
        public Button buttonObj { get; private set; }
        public Item GetItem => item;
        private Item item;

        private void Awake()
        {
            buttonObj = GetComponent<Button>();
        }

        public void SetItem(Item item)
        {
            this.item = item;
            slotText.text = GetItem.itemName;
        }

        public void SlotClick()
        {
            OnSlotClick?.Invoke(this);
        }
    }
}