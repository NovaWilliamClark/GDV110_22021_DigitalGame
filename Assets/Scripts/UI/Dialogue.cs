/*******************************************************************************************
*
*    File: Dialogue.cs
*    Purpose: Supplies the dialogue
*    Author: Joshua Stephens
*    Date: 25/10/2022
*
**********************************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public List<DialogueEntry> entries;
    public int currentPosition = 0;
}

[Serializable]
public class DialogueEntry
{
    [TextArea(3, 10)] public string sentence;
    public Vector3 position;

    public float holdDuration;
    public float fadeDuration;
}