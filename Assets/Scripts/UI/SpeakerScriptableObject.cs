using UnityEngine;

[CreateAssetMenu(fileName = "SpeakerScriptableObject", menuName = "Create Speaker")]
public class SpeakerScriptableObject : ScriptableObject
{
    public string speakerName;
    public Sprite speakerSprite;
    public AudioClip SpeakerAudioClip;
}
