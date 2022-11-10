/*******************************************************************************************
*
*    File: ItemUseRecipe.cs
*    Purpose: Inherits from ItemUse. Represents an area that requires multiple objects in the form of a recipe
*    Author: Sam Blakely
*    Date: 11/10/2022
*    Updated: 24/10/2022
*
**********************************************************************************************/

using System.Collections.Generic;
using UnityEngine;

public class ItemUseRecipe : ItemUse
{
    [SerializeField] private ItemRecipe recipe;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (recipe != null)
        {
            promptBox.gameObject.SetActive(true);
            //base.OnTriggerEnter2D(other);
            if (other.TryGetComponent<CharacterController>(out var player))
            {
                if (!recipe.RecipeConditionsMet(player.GetInventory.items))
                {
                    List<string> missingItems = recipe.GetMissingItems(player.GetInventory.items);
                    string str = missingItems[0];
                    for (int i = 1; i < missingItems.Count; i++)
                    {
                        str += $" & {missingItems[i]}";
                    }
                    messageText.text = $"You do not have {str}";
                }
                else
                {
                    if (useGenericMessage)
                    {
                        string str = recipe.requiredItems[0].itemName;
                        for (int i = 1; i < recipe.requiredItems.Count; i++)
                        {
                            str += $" & {recipe.requiredItems[i].itemName}";
                        }
                        promptMessage = $"Use {str}? - RMB";
                        messageText.text = promptMessage;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Recipe has not been set!");
        }
    }
    protected override void Interact(CharacterController cc)
    {
        if (recipe.RecipeConditionsMet(cc.GetInventory.items))
        {
            foreach (var item in recipe.requiredItems)
            {
                //cc.GetInventory.UseItem(item.itemID);
                Debug.Log($"{item.itemName} used!");
                data.state = InteractiveData.InteractionState.INACTIVE;
                gameObject.SetActive(false);
            }
        }
    }
}