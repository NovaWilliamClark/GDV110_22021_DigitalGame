using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class LightSwitch : InteractionPoint
{
    [FormerlySerializedAs("OnInteract")] public UnityEvent<bool> onInteract;
    
    [Header("Audio")]
    public AudioClip switchSfx;
    public float sfxVolume;
    
    [Header("Settings")]
    private bool interacted;
    public float interactCooldown = 0.5f;
    private bool active = false;
    
    protected override void Interact(CharacterController cc)
    {
        if (!interacted)
        {
            active = !active;
            interacted = true;
            canInteract = false;
            onInteract.Invoke(active);
            AudioManager.Instance.PlaySound(switchSfx,sfxVolume);
            Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
            StartCoroutine(WaitForCooldown());
        }
        
    }

    private IEnumerator WaitForCooldown()
    {
        yield return new WaitForSeconds(interactCooldown);
        interacted = false;
        canInteract = true;
        hasInteracted = false;
    }
    
    public override void SetInteractedState(object state)
    {
        if (state is not InteractionState st) return;    
        interacted = !st.interacted;
        canInteract = st.interacted;
        onInteract.Invoke(st.interacted);
    }
    
}
