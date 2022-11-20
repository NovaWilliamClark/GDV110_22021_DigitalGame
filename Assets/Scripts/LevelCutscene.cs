using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Character;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class LevelCutscene : MonoBehaviour
{
    // manages hooks for PlayableDirector and cutscene

    public CutsceneDialogue dialogue;
    private int dialogueIndex = 0;
    private PlayableDirector director;

    public UnityEvent Completed;

    private CharacterController player;
    private LevelController lvlController;

    [SerializeField] private string playerTrack = "Player";
    [SerializeField] private string playerModelTrack = "PlayerModel";
    [SerializeField] private string expression = "Expression";
    [SerializeField] private CinemachineVirtualCamera vcam;

        private bool sanityToggle = false;
        private bool flashlightOnBeforePlay;

        private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        lvlController = FindObjectOfType<LevelController>();
        lvlController.PlayerSpawned.AddListener(OnPlayerSpawned);
        director.Stop();
        gameObject.SetActive(false);
    }

    public void OnPlayerSpawned(CharacterController cc)
    {
        player = cc;
        var animator = player.GetComponent<Animator>();
        var modelAnimator = player.animator;
        foreach (var asset in director.playableAsset.outputs)
        {
            if (asset.streamName == playerTrack)
            {
                director.SetGenericBinding(asset.sourceObject,animator);
            }

            if (asset.streamName == playerModelTrack)
            {
                director.SetGenericBinding(asset.sourceObject,modelAnimator);
            }

            if (asset.streamName == expression)
            {
                director.SetGenericBinding(asset.sourceObject, cc.expressionAnimator);
            }
        }
        lvlController.PlayerSpawned.RemoveListener(OnPlayerSpawned);
    }
    
    private void OnDirectorStopped(PlayableDirector obj)
    {
        //Debug.Log("Director has stopped");
        if (player)
        {
            player.enabled = true;
            player.PlayerData.playingCutscene = true;
            player.ToggleActive(true);
            var sanity = player.GetComponent<CharacterSanity>();
            if (player.PlayerData.equipmentState.flashlightEquipped)
                UIHelpers.Instance.BatteryIndicator.Show(false);
            if (flashlightOnBeforePlay)
            {
                player.Equipment.ToggleFlashlight(true);
                flashlightOnBeforePlay = false;
            }
            if (!lvlController.safeZone)
            {
                sanity.Enable();
                sanity.AdjustDecreaseRate(0f, true);
            }
            
            player.SetAnimationControl();
        }
        Completed.Invoke();
        
    }

    private void PauseTimeline()
    {
        director.Pause();
    }

    public void WaitForInput()
    {
    }

    public void Play()
    {
        director.stopped += OnDirectorStopped;
        if (player)
        {
            var sanity = player.GetComponent<CharacterSanity>();
            player.PlayerData.playingCutscene = true;
            sanity.AdjustDecreaseRate(0f);
            if (player.PlayerData.equipmentState.flashlightEquipped)
                UIHelpers.Instance.BatteryIndicator.Hide();
            if (player.PlayerData.equipmentState.flashlightIsOn)
            {
                player.Equipment.ToggleFlashlight(false);
                flashlightOnBeforePlay = true;
            }
            sanity.Disable();
            player.enabled = false;
            //vcam.Follow = player.transform;
            player.ToggleActive(false);
            player.SetAnimationControl(true);
        }

        director.Play();
    }
    

    public void ShowCutsceneDialogue()
    {
        //var dialogue = GetComponent<>()
        //PauseTimeline();
        if (dialogue.entries.Count > 0)
        {
            if (dialogueIndex >= dialogue.entries.Count) return;
            CutsceneDialogueManager.Instance.ShowDialogue(dialogue.entries[dialogueIndex], OnSentenceComplete);
            dialogueIndex++;
        }
    }

    public void ToggleSanityMeter()
    {
        sanityToggle = !sanityToggle;
        UIHelpers.Instance.SanityMeter.ToggleVisibility(sanityToggle,1f, () =>
        {

            
        });
    }

    public void LoadLevel(LevelData_SO level)
    {
        UIHelpers.Instance.Fader.Fade(1f, .5f, () =>
        {
            TransitionManager.Instance.LoadScene(level.sceneName);
        });
    }

    public void MainMenu()
    {
        if (player.PlayerData.equipmentState.flashlightEquipped)
        {
            player.Equipment.FlashlightVisual.SetActive(false);
            UIHelpers.Instance.BatteryIndicator.Hide();
        }
        
        // This code resets your equipment state when you go to the main menu,
        // need to clarify if we want to start anew or continue game from menu
        // data.equipmentState = EquipmentState.Reset();
        // inventory.items.Clear();
        // inventory.slots.Clear();
        
        UIHelpers.Instance.SanityMeter.UnsetPlayer();
        TransitionManager.Instance.LoadScene("MainMenu");
    }
    
    public void ChangePlayerSortingLayer(int value)
    {
        if (!player) player = FindObjectOfType<CharacterController>();
        var sl = player.GetComponentInChildren<SortingModeChanger>();
        sl.SetSortingValue(value);
        Debug.Log(value);
    }

    public void ResetPlayerSortingLayer()
    {
        if (!player) player = FindObjectOfType<CharacterController>();
        var sl = player.GetComponentInChildren<SortingModeChanger>();
        sl.Reset();
        Debug.Log("Reset sorting layer");
    }

    private void OnSentenceComplete()
    {
        director.Resume();
    }
}