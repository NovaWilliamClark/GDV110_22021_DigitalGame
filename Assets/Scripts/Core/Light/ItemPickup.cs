/*******************************************************************************************
*
*    File: ItemPickup.cs
*    Purpose: Inherits from InteractionPoint. Holds an object for player pickup
*    Author: Sam Blakely
*    Date: 11/10/2022
*    Updated: 24/10/2022
*
**********************************************************************************************/

using Objects;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemPickup : InteractionPoint
{
    [SerializeField] private Item item;

    protected override void Interact(CharacterController cc)
    {
        cc.AddToInventory(item);
        Destroy(gameObject);
    }
}