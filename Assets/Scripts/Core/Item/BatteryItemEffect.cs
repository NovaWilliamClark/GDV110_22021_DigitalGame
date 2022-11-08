using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryItemEffect : ItemEffect
{
    public override void Use()
    {
        Debug.Log("Used battery");
        itemUseEvent.UsedEvent?.Invoke();
    }

    public BatteryItemEffect(ItemUseEvent useEvent) : base(useEvent) {}
}
