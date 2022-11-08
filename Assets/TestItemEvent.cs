using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestItemEvent : MonoBehaviour
{
    public bool playerInRange;

    public ItemUseEvent ItemUseToListenFor;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            ItemUseToListenFor.UsedEvent.AddListener(OnItemUse);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ItemUseToListenFor.UsedEvent.RemoveListener(OnItemUse);
        }
    }

    private void OnItemUse()
    {
        Debug.Log("I heard you use that item HEH");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        
    }
}
