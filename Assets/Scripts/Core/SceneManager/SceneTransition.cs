/*******************************************************************************************
*
*    File: SceneTransition.cs
*    Purpose: An area that upon a player entering, starts a transition to a new scene
*    Author: Sam Blakely
*    Date: 10/10/2022
*
**********************************************************************************************/

using UnityEngine;

namespace Objects
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SceneTransition : MonoBehaviour
    {
        [SerializeField] private string sceneToLoad;
        [SerializeField] private int spawnPointIndex;

        private void Awake()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<CharacterController>(out var player))
            {
                player.SetPersistentData();
                UIHelpers.Instance.Fader.Fade(1f, 1f, () =>
                {
                    TransitionManager.Instance.SetSpawnIndex(spawnPointIndex);
                    TransitionManager.Instance.LoadScene(sceneToLoad);
                });
            }
        }
    }
}