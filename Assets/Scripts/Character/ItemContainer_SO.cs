using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;

[CreateAssetMenu(menuName = "Create ItemContainer_SO", fileName = "ItemContainer_SO", order = 0)]
public class ItemContainer_SO : ScriptableObject
{
    public List<ItemData> initialItems;
    private List<ItemData> items = new();
    private List<ItemData> itemsTaken = new();

    public List<ItemData> ItemsTaken => itemsTaken;
    
    public List<ItemData> Items => items;

    private void OnEnable()
    {
        itemsTaken.Clear();
        items.Clear();
    }

    public void Init()
    {
        itemsTaken.Clear();
        items.Clear();
        foreach (var initialItem in initialItems)
        {
            items.Add(initialItem);
        }
    }

    public void SetItems()
    {
        foreach (var item in items.ToList())
        {
            if (itemsTaken.Contains(item))
            {
                items.Remove(item);
            }
        }
    }

    public bool AllItemsTaken => itemsTaken.Count == items.Count;

    public void SetToTaken(ItemData data)
    {
        //if (data)
       // ItemDatabase.Instance.GetOriginalItem(data)
        itemsTaken.Add(items.Find(x => x == data));
    }
}