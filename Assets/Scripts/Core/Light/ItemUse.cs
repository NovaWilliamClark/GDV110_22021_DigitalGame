/*******************************************************************************************
*
*    File: ItemUse.cs
*    Purpose: Inherits from InteractionPoint. Uses an inventory item on interaction and invokes an event
*    Author: Sam Blakely
*    Date: 24/10/2022
*    Updated: 
*
**********************************************************************************************/

using Objects;
using TMPro;
using UnityEngine;

public class ItemUse : InteractionPoint
{
    public InteractiveData GetData => data;
    [SerializeField] private Item requiredItem;
    [SerializeField] protected bool useGenericMessage = true;
    [SerializeField] protected InteractiveData data;
    protected TextMeshProUGUI messageText;

    protected override void Awake()
    {
        base.Awake();
        messageText = promptBox.GetComponentInChildren<TextMeshProUGUI>(true);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        
        if (requiredItem != null)
        {
            base.OnTriggerEnter2D(other);
            if (other.TryGetComponent<CharacterController>(out var player))
            {
                if (!player.GetInventory.HasItem(requiredItem.itemID))
                {
                    messageText.text = $"You do not have {requiredItem.itemName}";
                }
                else
                {
                    if (useGenericMessage)
                    {
                        promptMessage = $"Use {requiredItem.itemName}? - RMB";
                        messageText.text = promptMessage;
                    }
                }
                
            }
        }
        else
        {
            Debug.LogWarning("Required Item has not been set!");
        }
    }

    protected override void Interact(CharacterController cc)
    {
        if (!cc.GetInventory.HasItem(requiredItem.itemID)) return;
        
        //cc.GetInventory.UseItem(requiredItem.itemID);
        Debug.Log($"{requiredItem.itemName} used!");
        data.state = InteractiveData.InteractionState.INACTIVE;
        gameObject.SetActive(false);
    }
}