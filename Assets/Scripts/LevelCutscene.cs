using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class LevelCutscene : MonoBehaviour
{
    // manages hooks for PlayableDirector and cutscene

    public CutsceneDialogue dialogue;
    private PlayableDirector director;

    public UnityEvent Completed;

    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    private void Start()
    {
        director.stopped += OnDirectorStopped;
    }

    private void OnDirectorStopped(PlayableDirector obj)
    {
        Debug.Log("Director has stopped");
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
        director.Play();
    }

    public void ShowCutsceneDialogue()
    {
        //var dialogue = GetComponent<>()
        PauseTimeline();
        CutsceneDialogueManager.Instance.ShowDialogue(dialogue,OnSentenceComplete);
    }

    private void OnSentenceComplete()
    {
        director.Resume();
    }
}