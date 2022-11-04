/*******************************************************************************************
*
*    File: Item.cs
*    Purpose: Scriptable Object to hold data relating to an in-game object
*    Author: Sam Blakely
*    Date: 24/10/2022
*    Updated: 
*
**********************************************************************************************/

using System;
using UnityEngine;

namespace Objects
{
    
    [CreateAssetMenu(menuName = "Create Item", fileName = "Item", order = 0)]
    [Serializable]
    public class Item : ScriptableObject
    {
        public int itemID;
        public string itemName;
        public Sprite itemSprite;
        public bool hasBeenPickedUp;
        public bool disappearsOnCombination;
        public bool isSingleUse;
        private void OnEnable()
        {
            hasBeenPickedUp = false;
        }

        public virtual void Buff(Item buff)
        {
            Debug.Log($"Buffing {itemName} with {buff.itemName}");
        }
    }
    
}

