using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeakerScriptableObject", menuName = "Create Speaker")]
public class SpeakerScriptableObject : ScriptableObject
{
    public string speakerName;
    public Sprite speakerSprite;
    public AudioClip SpeakerAudioClip;
}
