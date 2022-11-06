using System.Collections.Generic;
using Objects;
using UnityEngine;

[CreateAssetMenu(menuName = "Create ItemContainer_SO", fileName = "ItemContainer_SO", order = 0)]
public class ItemContainer_SO : ScriptableObject
{
    public List<Item> items;
    private List<int> itemsTaken;

    public void SetItems()
    {
        foreach (var item in items)
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