using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightItemEffect : ItemEffect
{
    public override void Use()
    {
        Debug.Log("Used flashlight");
        itemUseEvent.UsedEvent?.Invoke();
    }

    public FlashlightItemEffect(ItemUseEvent useEvent) : base(useEvent)
    {
    }
}
