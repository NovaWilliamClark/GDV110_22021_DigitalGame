using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CutsceneDialogue
{
    public List<CutsceneDialogueEntry> entries = new();
}

[Serializable]
public class CutsceneDialogueEntry
{
    [TextArea(3, 10)]
    public string sentence;

    public SpeakerScriptableObject speaker;

    public float textSpeed;

    public float holdDuration;

    public FaceIconPosition iconPosition;
}

public enum FaceIconPosition
{
    Left,
    Right
}