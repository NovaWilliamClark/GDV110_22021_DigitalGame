using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Items/Create Item Event", fileName = "ItemEvent")]
public class ItemUseEvent : ScriptableObject
{
    public UnityEvent UsedEvent;
}
