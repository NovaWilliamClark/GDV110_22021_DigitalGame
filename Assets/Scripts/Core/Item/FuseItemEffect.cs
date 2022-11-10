using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseItemEffect : ItemEffect
{
    public override void Use(GameObject owner)
    {
        Debug.Log("Used fuse");
        itemUseEvent.UsedEvent?.Invoke();
    }

    public FuseItemEffect(ItemUseEvent useEvent) : base(useEvent)
    {
    }
}
