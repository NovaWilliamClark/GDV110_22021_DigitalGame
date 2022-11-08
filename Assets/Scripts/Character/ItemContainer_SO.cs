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
    private List<int> itemsTaken = new();

    public List<ItemData> Items => items;
    public void Init()
    {
        itemsTaken.Clear();
        items.Clear();
        foreach (var initialItem in initialItems)
        {
            ItemData so = ScriptableObject.Instantiate(initialItem) as ItemData;
            items.Add(so);
        }
    }

    public void SetItems()
    {
        foreach (var item in items.ToList())
        {
            if (itemsTaken.Contains(item.itemID))
            {
                items.Remove(item);
            }
        }
    }

    public void SetToTaken(int id)
    {
        itemsTaken.Add(items.Find(x => x.itemID == id).itemID);
    }
}