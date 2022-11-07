using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using UnityEngine;

[CreateAssetMenu(menuName = "Create ItemContainer_SO", fileName = "ItemContainer_SO", order = 0)]
public class ItemContainer_SO : ScriptableObject
{
    public List<Item> initialItems;
    private List<Item> items = new();
    private List<int> itemsTaken = new();

    public List<Item> Items => items;
    public void Init()
    {
        itemsTaken.Clear();
        items.Clear();
        foreach (var initialItem in initialItems)
        {
            Item so = ScriptableObject.Instantiate(initialItem) as Item;
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