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
        cutscene.Completed.AddListener(OnCutsceneDone);
    }

    public override void SetInteractedState(object state)
    {
        base.SetInteractedState(state);
        InteractionState istate = state as InteractionState;
        cutscene.gameObject.SetActive(!istate.interacted);
        gameObject.SetActive(!istate.interacted);
    }

    private void OnCutsceneDone()
    {
        // do some clean up
        Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
    }
}
