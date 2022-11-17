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
        hasInteracted = true;
        canInteract = false;
        Interacted?.Invoke(this, new InteractionState(persistentObject.Id) {interacted = true});
    }

    public override void SetInteractedState(object state)
    {
        base.SetInteractedState(state);
        if (state is not InteractionState st) return;
        active = st.interacted;
        Interaction.Invoke(st.interacted);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
}
