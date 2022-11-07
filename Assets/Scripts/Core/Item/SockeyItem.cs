using System.Collections;
using System.Collections.Generic;
using Audio;
using Objects;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Sockey Item", fileName = "SockeyItem", order = 0)]
public class SockeyItem : Item
{
    public AudioClip clip;

    public override void Use()
    {
        Debug.Log("Sockey Item used");
        AudioManager.Instance.PlaySound(clip);
    }
}
