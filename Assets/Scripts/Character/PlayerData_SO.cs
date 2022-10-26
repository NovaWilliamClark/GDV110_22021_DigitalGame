/*******************************************************************************************
*
*    File: PlayerData_SO.cs
*    Purpose: Holds data for the player that can persists between scenes
*    Author: Sam Blakely
*    Date: 19/10/2022
*
**********************************************************************************************/

using System;
using System.Collections.Generic;
using Character;
using Objects;
using UnityEngine;


[CreateAssetMenu(menuName = "Create PlayerData", fileName = "PlayerData", order = 0)]
public class PlayerData_SO : ScriptableObject
{
    [Header("--Sanity--")] 
    public float sanity = 100f;
    public float sanityGainRate = 0.025f;
    public float sanityLossRate = 0.03f;
    public List<Item> inventoryItems;

    private void OnEnable()
    {
        sanity = 100f;
        sanityGainRate = 0.025f;
        sanityLossRate = 0.03f;
        inventoryItems.Clear();
    }

    public void SetItems(Inventory inventory)
    {
        inventoryItems = inventory.items;
        /*foreach (var item in inventory.items)
        {
            if (inventoryItems.Contains(item))
            {
                continue;
            }
            inventoryItems.Add(item.GetItem);
        }*/
    }

    public void ResetData()
    {
        //sanity = 100f;
        //sanityGainRate = 0.025f;
        //sanityLossRate = 0.03f;
        //inventoryItems.Clear();
    }
}