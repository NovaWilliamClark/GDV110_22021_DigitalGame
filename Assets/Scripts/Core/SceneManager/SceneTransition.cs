/*******************************************************************************************
*
*    File: SceneTransition.cs
*    Purpose: An area that upon a player entering, starts a transition to a new scene
*    Author: Sam Blakely
*    Date: 10/10/2022
*
**********************************************************************************************/

using UnityEditor.SearchService;
using UnityEngine;

namespace Objects
{
    public class SceneTransition : InteractionPoint
    {
        [Header("Scene Transition")]
        [SerializeField] private string sceneToLoad;
        [SerializeField] private int spawnPointIndex;

        [SerializeField] private ItemData requiredItem;
        [SerializeField] private bool requiresItem;
        private bool hasItem;
        
        protected override void Interact(CharacterController cc)
        {
            if (!cc) return;
            cc.SetPersistentData();
            UIHelpers.Instance.Fader.Fade(1f, 1f, () =>
            {
                TransitionManager.Instance.SetSpawnIndex(spawnPointIndex);
                TransitionManager.Instance.LoadScene(sceneToLoad);
            });
        }
    }
}