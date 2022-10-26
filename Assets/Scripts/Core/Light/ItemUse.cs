/*******************************************************************************************
*
*    File: ItemUse.cs
*    Purpose: Inherits from InteractionPoint. Uses an inventory item on interaction and invokes an event
*    Author: Sam Blakely
*    Date: 24/10/2022
*    Updated: 
*
**********************************************************************************************/

using System;
using Objects;
using UnityEngine;

public class ItemUse : InteractionPoint
{
    public event Action OnItemUse;
    [SerializeField] private Item requiredItem;
    protected override void Interact(CharacterController cc)
    {
        if (!cc.GetInventory.HasItem(requiredItem.itemID)) return;
        
        cc.GetInventory.UseItem(requiredItem.itemID);
        Debug.Log($"{requiredItem.itemName} used!");
        OnItemUse?.Invoke();
        Destroy(gameObject);
    }
}