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
        //lvlController.PlayerSpawned.AddListener(OnPlayerSpawned);
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

    private void Start()   
    {
        director.stopped += OnDirectorStopped;
    }

    private void OnDirectorStopped(PlayableDirector obj)
    {
        Debug.Log("Director has stopped");
        player.enabled = true;
        player.ToggleMovement(true);
        player.GetComponent<CharacterSanity>().Enable();
        player.SetAnimationControl();
        player.GetComponent<CharacterSanity>().AdjustDecreaseRate(0f, true);
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
        player.GetComponent<CharacterSanity>().AdjustDecreaseRate(0f);
        
        player.enabled = false;
        //vcam.Follow = player.transform;
        player.ToggleMovement(false);
        player.SetAnimationControl(true);
        director.Play();
    }
    

    public void ShowCutsceneDialogue()
    {
        //var dialogue = GetComponent<>()
        //PauseTimeline();
        if (dialogue.entries.Count > 0)
        {
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

    private void OnSentenceComplete()
    {
        director.Resume();
    }
}