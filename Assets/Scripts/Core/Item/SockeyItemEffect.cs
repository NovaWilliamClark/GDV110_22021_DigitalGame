using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SockeyItemEffect : ItemEffect
{
    public override void Use(GameObject owner)
    {
        Debug.Log("Used socky");
        itemUseEvent.UsedEvent?.Invoke();
    }

    public SockeyItemEffect(ItemUseEvent useEvent) : base(useEvent)
    {
    }
}
