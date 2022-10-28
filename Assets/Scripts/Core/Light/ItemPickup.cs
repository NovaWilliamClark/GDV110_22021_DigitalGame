﻿/*******************************************************************************************
*
*    File: ItemPickup.cs
*    Purpose: Inherits from InteractionPoint. Holds an object for player pickup
*    Author: Sam Blakely
*    Date: 11/10/2022
*    Updated: 24/10/2022
*
**********************************************************************************************/

using System.Collections;
using Objects;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemPickup : InteractionPoint
{
    public Item GetItem => item;
    [SerializeField] private Item item;

    protected override void Interact(CharacterController cc)
    {
        cc.AddToInventory(item);
        item.hasBeenPickedUp = true;
        gameObject.SetActive(false);
    }
}