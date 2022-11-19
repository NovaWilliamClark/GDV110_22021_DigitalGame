/*******************************************************************************************
*
*    File: SceneTransition.cs
*    Purpose: An area that upon a player entering, starts a transition to a new scene
*    Author: Sam Blakely
*    Date: 10/10/2022
*
**********************************************************************************************/

using System;
using Core.SceneManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Objects
{
    public class SceneTransition : InteractionPoint
    {
        [Header("Scene Transition")]
        [SerializeField] protected LevelData_SO sceneToLoad;
        public string TargetScene => sceneToLoad.sceneName;
        [SerializeField] protected Vector2 spawnPosition = new Vector2(0,-12f);
        [SerializeField] protected PlayerSpawnPoint.FacingDirection spawnFacingDirection = PlayerSpawnPoint.FacingDirection.Right;

        public Vector2 SpawnPosition
        {
            get
            {
                return transform.position + (Vector3) spawnPosition;
            }
        } 
        public PlayerSpawnPoint.FacingDirection SpawnFacingDirection=> spawnFacingDirection;

        protected override void Interact(CharacterController cc)
        {
            if (!cc) return;
            cc.SetPersistentData();
            Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
            UIHelpers.Instance.Fader.Fade(1f, 1f, () =>
            {
               
                TransitionManager.Instance.LoadScene(sceneToLoad.sceneName);
            });
        }

        protected new virtual void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(SpawnPosition,Vector3.one);
            base.OnDrawGizmos();
        }
    }
}