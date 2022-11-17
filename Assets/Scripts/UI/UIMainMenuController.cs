using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class UIMainMenuController : MonoBehaviour
{
    private PlayableDirector timeline;

    private UnityAction onCompleteCallback;
    
    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
        Cursor.visible = true;
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
