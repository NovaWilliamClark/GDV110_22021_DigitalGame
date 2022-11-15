using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class CharacterFootsteps : MonoBehaviour
{
    // TODO: Floor type will have different sound
    // Detect the type of floor via raycast - floor tiles should hold this type
    
    public List<AudioClip> FootstepClips;

    public void OnFootstep()
    {
        if (!Application.isPlaying) return;
        var clip = FootstepClips[Random.Range(0, FootstepClips.Capacity)];
        AudioManager.Instance.PlaySound(clip,0.2f);
    }
}
