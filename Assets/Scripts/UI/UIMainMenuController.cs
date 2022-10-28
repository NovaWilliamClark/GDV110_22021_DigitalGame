using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class UIMainMenuController : MonoBehaviour
{
    private PlayableDirector timeline;

    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
    }

    public void PauseTimeline()
    {
        timeline.Pause();
    }

    public void ResumeTimeline()
    {
        timeline.Resume();
    }
}
