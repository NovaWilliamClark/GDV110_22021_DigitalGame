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
    [SerializeField] private CinemachineVirtualCamera vcam;

        private bool sanityToggle = false;

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
        }
        lvlController.PlayerSpawned.RemoveListener(OnPlayerSpawned);
    }
    
    private void OnDirectorStopped(PlayableDirector obj)
    {
        //Debug.Log("Director has stopped");
        if (player)
        {
            player.enabled = true;
            player.ToggleActive(true);
            var sanity = player.GetComponent<CharacterSanity>();
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
            sanity.AdjustDecreaseRate(0f);
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