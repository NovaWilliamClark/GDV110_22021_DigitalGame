/*******************************************************************************************
*
*    File: RecipeDatabase.cs
*    Purpose: Inherits from scriptable object. Holds all the recipes in the game
*    Author: Sam Blakely
*    Updated: 24/10/2022
*
**********************************************************************************************/

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create RecipeDataBase", fileName = "RecipeDataBase", order = 0)]
public class RecipeDataBase : ScriptableObject
{
    public List<ItemRecipe> recipes;
}