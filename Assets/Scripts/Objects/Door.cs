using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class Door : InteractionPoint
{
    [Header("Scene Transition")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private int spawnPointIndex;

    [Header("SFX")] 
    [SerializeField] private AudioClip sfxToPlay;
    [SerializeField] private float sfxVolume;
    
    private bool interacted;

    protected override void Interact(CharacterController cc)
    {
        if (!interacted)
        {
            interacted = true;
            cc.SetPersistentData();
            AudioManager.Instance.PlaySound(sfxToPlay, sfxVolume);
            UIHelpers.Instance.Fader.Fade(1f, sfxToPlay.length + 0.1f, () =>
            {
                TransitionManager.Instance.SetSpawnIndex(spawnPointIndex);
                TransitionManager.Instance.LoadScene(sceneToLoad);
            });
        }
    }
}