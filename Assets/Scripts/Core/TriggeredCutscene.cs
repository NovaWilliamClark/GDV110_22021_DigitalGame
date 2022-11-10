using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TriggeredCutscene : InteractionPoint
{
    [SerializeField] private LevelCutscene cutscene;
     
    protected override void Interact(CharacterController cc)
    {
        cutscene.Play();
    }

    public override void SetInteractedState()
    {
        hasInteracted = true;
    }

    private void OnCutsceneDone(PlayableDirector obj)
    {
        // do some clean up
    }
}
