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
using UnityEngine.UI;

namespace Objects
{
    
    [CreateAssetMenu(menuName = "Create Item", fileName = "Item", order = 0)]
    [Serializable]
    public class Item : ScriptableObject
    {
        public int itemID;
        public string itemName;
        public Image itemImage;
        public bool hasBeenPickedUp = false;
    }
}

