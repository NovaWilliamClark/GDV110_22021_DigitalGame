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
using Unity.XR.OpenVR;
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
        [HideInInspector, SerializeField] private Item itemRef; // original item for object to refer back to
        
        //https://forum.unity.com/threads/solved-checking-equality-between-scriptableobject-instances.519270/
        
        public virtual void OnEnable()
        {
            hasBeenPickedUp = false;
        }

        public virtual void Buff(Item buff)
        {
            Debug.Log($"Buffing {itemName} with {buff.itemName}");
        }

        // Creates an instance of the scriptable object so we're not manipulating the original
        public static Item CreateInstance(Item original)
        {
            Item instance = Instantiate(original);
            instance.itemRef = original;
            return instance;
        }

        public bool IsInstanceOf(Item item)
        {
            if (itemRef == item)
            {
                return true;
            }

            return false;
        }

        public virtual void Use() {}
    }
    
}

