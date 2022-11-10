using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TriggeredCutscene : InteractionPoint
{
    [SerializeField] private PlayableDirector cutscene;
     
    protected override void Interact(CharacterController cc)
    {
        cutscene.Play();
        cutscene.stopped += OnCutsceneDone;
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
