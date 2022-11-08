using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FlashlightItemEffect : ItemEffect
{
    public override void Use()
    {
        Debug.Log("Used flashlight");
        itemUseEvent.UsedEvent?.Invoke();
    }

    public override void Reload(GameObject player)
    {
        // do the thing
        var equipment = player.GetComponent<CharacterEquipment>();
        equipment.ReloadFlashlight();

    }

    public FlashlightItemEffect(ItemUseEvent useEvent) : base(useEvent)
    {
    }
}
