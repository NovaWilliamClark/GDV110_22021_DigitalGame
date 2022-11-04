/*******************************************************************************************
*
*    File: ItemRecipe.cs
*    Purpose: Inherits from scriptable object. Represents a recipe containing multiple other objects to create another object
*    Author: Sam Blakely
*    Updated: 2/11/2022
*
**********************************************************************************************/

using System.Collections.Generic;
using Objects;
using UnityEngine;

[CreateAssetMenu(menuName = "Create ItemRecipe", fileName = "ItemRecipe", order = 0)]
public class ItemRecipe : ScriptableObject
{
    public Item result;
    public List<Item> requiredItems;
    

    public bool RecipeConditionsMet(List<Item> inventory)
    {
        int requirementsMet = 0;
        foreach (var item in inventory)
        {
            if (requiredItems.Contains(item))
            {
                requirementsMet++;
            }
        }

        return requirementsMet == requiredItems.Count;
    }

    public List<string> GetMissingItems(List<Item> inventory)
    {
        List<string> missingItems = new List<string>();

        foreach (var item in requiredItems)
        {
            if (!inventory.Contains(item))
            {
                missingItems.Add(item.itemName);
            }
        }

        return missingItems;
    }
}