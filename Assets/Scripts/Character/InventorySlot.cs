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

namespace Character
{
    [Serializable]
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI slotText;
        public Item GetItem => item;
        private Item item;

        public void SetItem(Item item)
        {
            this.item = item;
            slotText.text = GetItem.itemName;
        }
    }
}