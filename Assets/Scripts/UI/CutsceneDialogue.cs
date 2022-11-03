using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CutsceneDialogue
{
    [TextArea(3, 10)]
    public string[] sentences;

    public SpeakerScriptableObject speaker;

    public float textSpeed;
}
