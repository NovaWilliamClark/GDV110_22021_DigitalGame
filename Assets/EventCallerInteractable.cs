using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Events;

public class EventCallerInteractable : InteractionPoint
{
    public UnityEvent<bool> Interaction;
    private bool active = false;
    
    protected override void Interact(CharacterController cc)
    {
        base.Interact(cc);
        active = !active;
        Interaction?.Invoke(active);
    }

    public override void SetInteractedState()
    {
        active = !active;
        Interaction.Invoke(active);
    }
}
