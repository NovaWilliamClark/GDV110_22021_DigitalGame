/*******************************************************************************************
*
*    File: ItemPickup.cs
*    Purpose: Inherits from InteractionPoint. Holds an object for player pickup
*    Author: Sam Blakely
*    Date: 11/10/2022
*    Updated: 24/10/2022
*
**********************************************************************************************/

using System;
using Objects;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemPickup : InteractionPoint
{
    public Item GetItem => item;
    [SerializeField] private Item item;
    private SpriteRenderer renderer;

    protected override void Awake()
    {
        base.Awake();
        renderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        if (item != null)
        {
            if (item.itemSprite != null && renderer != null && renderer.sprite == null)
            {
                renderer.sprite = item.itemSprite;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (item != null)
        {
            if (promptMessage == String.Empty)
            {
                promptMessage = $"Pickup {item.itemName} - RMB";
            }
            base.OnTriggerEnter2D(other);
        }
        else
        {
            Debug.LogWarning("Item has not been set on pickup!");
        }
    }

    protected override void Interact(CharacterController cc)
    {
        cc.AddToInventory(item);
        item.hasBeenPickedUp = true;
        gameObject.SetActive(false);
    }
}