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
using UnityEngine.Serialization;

namespace Objects
{
    [CreateAssetMenu(menuName = "Items/Create Item", fileName = "Item Data", order = 0)]
    [Serializable]
    public class ItemData : ScriptableObject
    {
        public int itemID;
        public string itemName;
        public Sprite itemSprite;
        public bool hasBeenPickedUp;
        public bool disappearsOnCombination;
        public bool isSingleUse;
        public bool reloadable;
        public ItemData itemToReload;
        [FormerlySerializedAs("itemRef")] [HideInInspector, SerializeField] private ItemData itemDataRef; // original item for object to refer back to

        [FormerlySerializedAs("Effect")] public ItemTypeEnum typeEnum;
        
        //https://forum.unity.com/threads/solved-checking-equality-between-scriptableobject-instances.519270/
        
        public virtual void OnEnable()
        {
            hasBeenPickedUp = false;
        }

        public virtual void Buff(ItemData buff)
        {
            Debug.Log($"Buffing {itemName} with {buff.itemName}");
        }

        // Creates an instance of the scriptable object so we're not manipulating the original
        public static ItemData CreateInstance(ItemData original)
        {
            ItemData instance = Instantiate(original);
            instance.itemDataRef = original;
            return instance;
        }

        public bool IsInstanceOf(ItemData itemData)
        {
            if (itemDataRef == itemData)
            {
                return true;
            }

            return false;
        }

        public ItemData GetItemRef()
        {
            return itemDataRef;
        }

        public virtual void Use() {}
    }
    
}

